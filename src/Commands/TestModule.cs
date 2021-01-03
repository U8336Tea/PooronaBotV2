using System.Threading.Tasks;

using Discord.Commands;

namespace PooronaBot.Commands
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task TestCommandAsync()
        {
            await ReplyAsync("yes");
        }
    }
}