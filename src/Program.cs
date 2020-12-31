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
        static void Main(string[] args) => RealMain().GetAwaiter().GetResult();

        static async Task RealMain()
        {
            IConfiguration config = new EnvironmentConfiguration();

            // Initialize client
            var client = new DiscordSocketClient();
            client.Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, config.GetString("token"));
            await client.StartAsync();

            // Initialize commands
            var commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
            });

            commands.Log += LogAsync;

            var commandHandler = new CommandHandler(client, commands);
            await commandHandler.InstallCommandsAsync();

            // Initialize Infector
            var guild = client.GetGuild(config.GetID("guild"));
            var virusRole = guild.GetRole(config.GetID("infected-role"));
            var deadRole = guild.GetRole(config.GetID("dead-role"));
            var susceptible = config.GetIDList("susceptible-roles");
            var limit = config.GetInt("infection-limit");
            Infector.CreateInstance(guild, virusRole, deadRole, susceptible, limit);

            await Task.Delay(-1);
        }
        
        private static Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
