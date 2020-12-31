using System;
using System.Threading.Tasks;

using PooronaBot.Exceptions;
using PooronaBot.Commands.Preconditions;

using Discord;
using Discord.Commands;

namespace PooronaBot.Commands
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task TestCommandAsync()
        {
            await ReplyAsync("bababa");
        }
    }
}