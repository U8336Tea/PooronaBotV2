using System;
using System.Threading.Tasks;

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
            IConfiguration configuration = new EnvironmentConfiguration();

            // Initialize client
            var client = new DiscordSocketClient();
            client.Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, configuration["Token"]);
            await client.StartAsync();

            // Initialize commands
            var commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
            });

            commands.Log += LogAsync;

            var commandHandler = new CommandHandler(client, commands);
            await commandHandler.InstallCommandsAsync();

            await Task.Delay(-1);
        }
        
        private static Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
