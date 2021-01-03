using System;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

using StackExchange.Redis;

namespace PooronaBot
{
    public class Scheduler
    {
        public int DeathHours;

        public double InfectInterval {
            get => _infectTimer.Interval;
            set => _infectTimer.Interval = value;
        }

        private Timer _infectTimer;
        private Timer _killTimer;
        private Timer _pruneTimer;
        private IDiscordClient _client;
        private IGuild _guild;
        private ConnectionMultiplexer _databaseConnection;

        // TODO: There's probably a better way to do this.
        public static Scheduler Instance {get; private set;}

        private Scheduler(double infectInterval, IDiscordClient client)
        {
            _infectTimer = new Timer(infectInterval);
            _infectTimer.AutoReset = true;
            _infectTimer.Elapsed += InfectTimerElapsed;
            _infectTimer.Start();

            _client = client;
        }

        private Scheduler(
            double infectInterval,
            IDiscordClient client,
            IGuild guild,
            int deathHours,
            ConnectionMultiplexer databaseConnection)
            : this(infectInterval, client)
        {
            _guild = guild;
            DeathHours = deathHours;
            _databaseConnection = databaseConnection;

            _killTimer = new Timer(30 * 60 * 1000);
            _killTimer.AutoReset = true;
            _killTimer.Elapsed += KillTimerElapsed;
            _killTimer.Start();

            PruneTimerElapsed(null, null);
            _pruneTimer = new Timer(3 * 60 * 60 * 1000);
            _pruneTimer.AutoReset = true;
            _pruneTimer.Elapsed += PruneTimerElapsed;
            _pruneTimer.Start();
        }

        public static Scheduler CreateInstance(
            double infectInterval,
            IDiscordClient client,
            IGuild guild,
            int deathHours = -1,
            ConnectionMultiplexer databaseConnection = null)
        {
            if (Instance != null) return Instance;

            if (databaseConnection == null) Instance = new Scheduler(infectInterval, client);
            else Instance = new Scheduler(infectInterval, client, guild, deathHours, databaseConnection);

            return Instance;
        }

        private void InfectTimerElapsed(object source, ElapsedEventArgs e)
        {
            Infector.Instance.InfectRandom().GetAwaiter().GetResult();
        }

        private void KillTimerElapsed(object source, ElapsedEventArgs e)
        {
            var database = _databaseConnection.GetDatabase();
            var users = _guild.GetUsersAsync(CacheMode.AllowDownload).GetAwaiter().GetResult();
            var potentials = Infector.Instance.ListInfected().GetAwaiter().GetResult();

            foreach (var user in potentials) {
                var info = database.HashGet("deaths", user.Id);

                // If the user is not in the database, kill them.
                DateTime deathTime = DateTime.UnixEpoch;
                if (info != RedisValue.Null) deathTime = DateTime.Parse(info).AddHours(DeathHours);
                if (DateTime.Now >= deathTime) {
                    Infector.Instance.Kill(user).GetAwaiter().GetResult();
                    database.HashDelete("deaths", user.Id);
                }
            }

            // Save Redis databases
            foreach (var endpoint in _databaseConnection.GetEndPoints())
            {
                var server = _databaseConnection.GetServer(endpoint);
                server.Save(SaveType.BackgroundSave);
            }
        }

        private void PruneTimerElapsed(object source, ElapsedEventArgs e)
        {
            var database = _databaseConnection.GetDatabase();
            var deaths = database.HashGetAll("deaths");
            
            foreach (var death in deaths) {
                var id = ulong.Parse(death.Name);
                var user = _guild.GetUserAsync(id, CacheMode.AllowDownload).GetAwaiter().GetResult();
                if (user == null) {
                    database.HashDelete("deaths", id);
                }
            }
        }
    }
}