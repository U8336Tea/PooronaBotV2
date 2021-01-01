using System.Threading.Tasks;

using PooronaBot.Config;

using Discord;
using Discord.Commands;

namespace PooronaBot.Commands
{
    [Group("admin")]
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        // TODO: Allow configuration to be passed in, for possible configuration file, etc.
        private IConfiguration _config = new EnvironmentConfiguration();

        [Command("changeinterval")]
        [Summary("Changes the infection interval.")]
        public async Task ChangeIntervalAsync([Summary("The new interval in milliseconds.")] double interval)
        {
            Scheduler.Instance.InfectInterval = interval;
            _config.Set("infection-interval", interval);
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }

        [Command("changedeathtime")]
        [Summary("Changes the time it takes for a user to die.")]
        public async Task ChangeDeathTimeAsync([Summary("The death time in hours.")] int time)
        {
            Scheduler.Instance.DeathHours = time;
            _config.Set("death-hours", time);
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }
    }
}