using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace PooronaBot.Commands.Preconditions
{
    public class RequireRichAttribute : PreconditionAttribute
    {
        // TODO: Have this reference a config file instead.
        private readonly ulong _roleID;

        public RequireRichAttribute(ulong roleID) => _roleID = roleID;

        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            if (!(context.User is IGuildUser user)) {
                return Task.FromResult(PreconditionResult.FromError("Don't DM me bitch."));
            }

            return Task.FromResult(user.RoleIds.Contains(_roleID) ?
                PreconditionResult.FromSuccess() :
                PreconditionResult.FromError("Try again when you aren't p*or <:cringe:793938801088659479>"));
        }
    }
}