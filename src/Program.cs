using System;
using System.Threading.Tasks;

using PooronaBot.Config;
using PooronaBot.Commands;

using StackExchange.Redis;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace PooronaBot
{
    class Program
    {
        static DiscordSocketClient _client;
        static void Main(string[] args) => RealMain().GetAwaiter().GetResult();

        static async Task RealMain()
        {
            IConfiguration config = new EnvironmentConfiguration();

            // Initialize client
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,

                GatewayIntents = GatewayIntents.Guilds |
                                GatewayIntents.GuildMembers |
                                GatewayIntents.GuildMessages,
            });
            
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;

            await _client.LoginAsync(TokenType.Bot, config.GetString("TOKEN"));
            await _client.StartAsync();

            // Initialize commands
            var commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
            });

            commands.Log += LogAsync;

            var commandHandler = new CommandHandler(_client, commands);
            await commandHandler.InstallCommandsAsync();

            await Task.Delay(-1);
        }
        
        private static Task ReadyAsync() {
            IConfiguration config = new EnvironmentConfiguration();

            // Initialize Infector
            var guild = _client.GetGuild(config.GetID("GUILD"));
            var virusRole = guild.GetRole(config.GetID("INFECTED_ROLE"));
            var deadRole = guild.GetRole(config.GetID("DEAD_ROLE"));
            var curedRole = guild.GetRole(config.GetID("CURED_ROLE"));
            var susceptible = config.GetIDList("SUSCEPTIBLE_ROLES");
            var limit = config.GetInt("INFECTION_LIMIT");
            var redisURL = config.GetString("REDIS_URL");
            ConnectionMultiplexer connection = null;

            if (!string.IsNullOrEmpty(redisURL)) connection = ConnectionMultiplexer.Connect(redisURL);

            Infector.CreateInstance(_client, guild, virusRole, deadRole, curedRole, susceptible, limit, connection);
            
            var interval = config.GetInt("INFECTION_INTERVAL");
            var deathHours = config.GetInt("DEATH_HOURS");
            Scheduler.CreateInstance(interval, _client, guild, virusRole.Id, deathHours, connection);

            return Task.CompletedTask;
        }

        private static Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
