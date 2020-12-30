using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace PooronaBot.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += MessageRecievedAsync;
            _commands.CommandExecuted += CommandExecutedAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        }

        private async Task MessageRecievedAsync(SocketMessage message)
        {
            if (!(message is SocketUserMessage userMessage)) return;
            if (userMessage.Source != MessageSource.User) return;

            int pos = 0;
            if (!userMessage.HasStringPrefix("!!", ref pos)) return;

            var context = new SocketCommandContext(_client, userMessage);
            await _commands.ExecuteAsync(context, pos, null);
        }

        private async Task CommandExecutedAsync(
            Optional<CommandInfo> command,
            ICommandContext context,
            IResult result)
        {
            if (!command.IsSpecified) return;
            if (result.IsSuccess) return;
            if (string.IsNullOrEmpty(result.ErrorReason)) return;

            await context.Message.AddReactionAsync(new Emoji("❌"));
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}