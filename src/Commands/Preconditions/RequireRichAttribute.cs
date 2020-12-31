using System;
using System.Linq;
using System.Threading.Tasks;

using PooronaBot.Config;

using Discord;
using Discord.Commands;

namespace PooronaBot.Commands.Preconditions
{
    public class RequireRichAttribute : PreconditionAttribute
    {
        private IConfiguration _config = new EnvironmentConfiguration();

        public RequireRichAttribute() {  }

        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var roleID = _config.GetID("rich-role");
            if (!(context.User is IGuildUser user)) {
                return Task.FromResult(PreconditionResult.FromError("Don't DM me bitch."));
            }

            return Task.FromResult(user.RoleIds.Contains(roleID) ?
                PreconditionResult.FromSuccess() :
                PreconditionResult.FromError("Try again when you aren't p*or <:cringe:793938801088659479>"));
        }
    }
}