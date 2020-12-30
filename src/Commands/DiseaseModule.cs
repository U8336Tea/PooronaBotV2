using System;
using System.Threading.Tasks;

using PooronaBot.Exceptions;
using PooronaBot.Commands.Preconditions;

using Discord;
using Discord.Commands;

namespace PooronaBot.Commands
{
    [RequireRich(776680578254766101)]
    [RequireContext(ContextType.Guild)]
    public class DiseaseModule : ModuleBase<SocketCommandContext> 
    {
        private readonly Infector _infector = Infector.Instance;

        [Command("infect")]
        [Summary("Infects someone.")]
        public async Task InfectUserAsync([Summary("The user to infect.")] IGuildUser user)
        {
            try {
                await _infector.Infect(user);
                await Context.Message.AddReactionAsync(new Emoji("✅"));
            } catch (LimitException) {
                await Context.Message.AddReactionAsync(Emote.Parse("<:virgin:793938832868900884>"));
                await ReplyAsync("Stop infecting so many people, you greedy asshole.");
            }
        }

        [Command("disinfect")]
        [Summary("Disinfects someone.")]
        public async Task DisinfectUserAsync([Summary("The user to disinfect.")] IGuildUser user)
        {
            await _infector.Disinfect(user);
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }

        [Command("kill")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Summary("Kills someone.")]
        public async Task KillUserAsync([Summary("The user to kill.")] IGuildUser user)
        {
            await _infector.Kill(user);
        }
    }
}