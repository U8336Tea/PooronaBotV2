using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using PooronaBot.Config;
using PooronaBot.Commands;

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

            await _client.LoginAsync(TokenType.Bot, config.GetString("token"));
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
            var guild = _client.GetGuild(config.GetID("guild"));
            var virusRole = guild.GetRole(config.GetID("infected-role"));
            var deadRole = guild.GetRole(config.GetID("dead-role"));
            var curedRole = guild.GetRole(config.GetID("cured-role"));
            var susceptible = config.GetIDList("susceptible-roles");
            var limit = config.GetInt("infection-limit");
            Infector.CreateInstance(guild, virusRole, deadRole, curedRole, susceptible, limit);

            return Task.CompletedTask;
        }

        private static Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
