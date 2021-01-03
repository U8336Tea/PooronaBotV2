using System.Threading.Tasks;

using PooronaBot.Exceptions;
using PooronaBot.Commands.Preconditions;

using Discord;
using Discord.Commands;

namespace PooronaBot.Commands
{
    [RequireRich]
    [RequireContext(ContextType.Guild)]
    public class DiseaseModule : ModuleBase<SocketCommandContext> 
    {
        private readonly Infector _infector = Infector.Instance;

        [Command("infect")]
        [Summary("Infects someone.")]
        public async Task InfectUserAsync(
            [Summary("The user to infect.")]
            [Remainder]
            IGuildUser user)
        {
            try {
                var perpetrator = Context.User;
                await _infector.Infect(user, $"Infected by {perpetrator.Username}#{perpetrator.Discriminator}");
                await Context.Message.AddReactionAsync(new Emoji("✅"));
            } catch (LimitException) {
                await Context.Message.AddReactionAsync(Emote.Parse("<:virgin:793938832868900884>"));
                await ReplyAsync("Don't you have anything better to do than infect people all day?");
            } catch (CuredException e) {
                await Context.Message.AddReactionAsync(Emote.Parse("<:libertycries:794102085356355584>"));
                await ReplyAsync(e.Message);
            }
        }

        [Command("disinfect")]
        [Summary("Disinfects someone.")]
        public async Task DisinfectUserAsync(
            [Summary("The user to disinfect.")]
            [Remainder]
            IGuildUser user)
        {
            var perpetrator = Context.User;
            await _infector.Disinfect(user, $"Disinfected by {perpetrator.Username}#{perpetrator.Discriminator}");
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }

        [Command("kill")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Summary("Kills someone.")]
        public async Task KillUserAsync(
            [Summary("The user to kill.")]
            [Remainder]
            IGuildUser user)
        {
            await _infector.Kill(user);
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }
    }
}