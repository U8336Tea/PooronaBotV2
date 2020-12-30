using System;
using System.Threading.Tasks;

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
            // Initialize client
            var client = new DiscordSocketClient();
            client.Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, "MzkyODUzNTAzMDAzMDY2Mzcx.Wjm_VQ.v6a2tSZ9rBhWmfwwhUzfWFO8CkQ");
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
