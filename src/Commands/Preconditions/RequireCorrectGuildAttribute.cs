using System;
using System.Linq;
using System.Threading.Tasks;

using PooronaBot.Config;

using Discord;
using Discord.Commands;

namespace PooronaBot.Commands.Preconditions
{
    public class RequireCorrectGuildAttribute : PreconditionAttribute
    {
        // TODO: Allow configuration to be passed in, for possible configuration file, etc.
        private IConfiguration _config = new EnvironmentConfiguration();

        public RequireCorrectGuildAttribute() {  }

        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context,
            CommandInfo command,
            IServiceProvider services)
        {
            var guildID = _config.GetID("GUILD");
            if (context.Guild == null) {
                return Task.FromResult(PreconditionResult.FromError("Don't DM me bitch."));
            }

            return Task.FromResult(context.Guild.Id == guildID ?
                PreconditionResult.FromSuccess() :
                PreconditionResult.FromError("Don't pretend to be admin <:cringe:793938801088659479>"));
        }
    }
}