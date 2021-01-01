using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using PooronaBot.Exceptions;

using Discord;
using Discord.Net;
using Discord.WebSocket;

using StackExchange.Redis;

namespace PooronaBot
{
    class Infector
    {
        // TODO: There's probably a better way to do this.
        public static Infector Instance {get; private set;}
        
        private IGuild _guild;
        private IRole _virusRole;
        private IRole _deadRole;
        private IRole _curedRole;
        private IList<ulong> _susceptibleRoleIDs;
        private int _limit;
        private Random _random = new Random();
        private ConnectionMultiplexer _databaseConnection;

        private Infector(
            DiscordSocketClient client,
            IGuild guild,
            IRole virusRole,
            IRole deadRole,
            IRole curedRole,
            IList<ulong> susceptibleRoleIDs,
            int limit,
            ConnectionMultiplexer databaseConnection = null)
        {
            _guild = guild;
            _virusRole = virusRole;
            _deadRole = deadRole;
            _curedRole = curedRole;
            _susceptibleRoleIDs = susceptibleRoleIDs;
            _limit = limit;
            _databaseConnection = databaseConnection;

            client.GuildMemberUpdated += RoleChanged;
        }

        public static Infector CreateInstance(
            DiscordSocketClient client,
            IGuild guild,
            IRole virusRole,
            IRole deadRole,
            IRole curedRole,
            IList<ulong> susceptibleRoleIDs,
            int limit,
            ConnectionMultiplexer databaseConnection = null)
        {
            if (Instance != null) return Instance;

            Instance = new Infector(client, guild, virusRole, deadRole, curedRole, susceptibleRoleIDs, limit, databaseConnection);
            return Instance;
        }
        public async Task Infect(IGuildUser user)
        {
            var members = await _guild.GetUsersAsync(CacheMode.AllowDownload);
            int numInfected = 
               (from member in members
                where member.RoleIds.Contains(_virusRole.Id)
                select member).Count();

            if (numInfected >= _limit) throw new LimitException(_limit, numInfected);
            if (user.RoleIds.Contains(_curedRole.Id)) throw new CuredException();
            await user.AddRoleAsync(_virusRole);

            try {
                await user.SendMessageAsync($"You have been infected with the virus!");
            } catch (HttpException e) when (e.DiscordCode == 50007) {  }

            if (_databaseConnection == null) return;

            var hours = Scheduler.Instance.DeathHours;
            try {
                await user.SendMessageAsync($"You will die within {hours} hours.");
            } catch (HttpException e) when (e.DiscordCode == 50007) {  }

            var database = _databaseConnection.GetDatabase();
            var pair = new HashEntry(user.Id, DateTime.Now.ToString());
            database.HashSet("deaths", new HashEntry[1]{pair});
        }

        public async Task Disinfect(IGuildUser user)
        {
            await user.RemoveRoleAsync(_virusRole);

            if (_databaseConnection == null) return;
            var database = _databaseConnection.GetDatabase();

            // Unschedule the user's death.
            database.HashDelete("deaths", user.Id);
        }

        public async Task Kill(IGuildUser user)
        {
            await Disinfect(user);
            await user.AddRoleAsync(_deadRole);
        }

        public async Task InfectRandom()
        {
            var members = await _guild.GetUsersAsync(CacheMode.AllowDownload);
            
            var eligible = 
                from member in members
                where member.RoleIds.Intersect(_susceptibleRoleIDs).Count() > 0
                select member;

            var eligibleArray = eligible.ToArray();
            var randomUser = eligibleArray[_random.Next(eligibleArray.Count())];
            await Infect(randomUser);
        }

        private async Task RoleChanged(SocketGuildUser oldUser, SocketGuildUser newUser)
        {
            if (oldUser.Roles == newUser.Roles) return;
            if (oldUser.Roles.Contains(_virusRole) && !newUser.Roles.Contains(_virusRole))
            {
                await Disinfect(newUser);
            }
        }
    }
}