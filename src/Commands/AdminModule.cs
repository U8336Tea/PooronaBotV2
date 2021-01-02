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
            _config.Set("INFECTION_INTERVAL", interval);
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }

        [Command("changedeathtime")]
        [Summary("Changes the time it takes for a user to die.")]
        public async Task ChangeDeathTimeAsync([Summary("The death time in hours.")] int time)
        {
            Scheduler.Instance.DeathHours = time;
            _config.Set("DEATH_HOURS", time);
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }

        [Command("changeinfectlimit")]
        [Summary("Changes the number of people who can be infected.")]
        public async Task ChangeInfectLimitAsync([Summary("The new limit.")] int limit)
        {
            Infector.Instance.InfectLimit = limit;
            _config.Set("INFECTION_LIMIT", limit);
            await Context.Message.AddReactionAsync(new Emoji("✅"));
        }
    }
}