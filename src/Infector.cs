using System.Linq;
using System.Threading.Tasks;

using PooronaBot.Exceptions;

using Discord;

namespace PooronaBot
{
    class Infector
    {
        // TODO: There's probably a better way to do this.
        public static Infector Instance {get; private set;}
        
        private IDiscordClient _client;
        private IRole _virusRole;
        private IRole _deadRole;
        private int _limit;

        private Infector(
            IDiscordClient client,
            IRole virusRole,
            IRole deadRole,
            int limit)
        {
            _client = client;
            _virusRole = virusRole;
            _deadRole = deadRole;
            _limit = limit;
        }

        public Infector CreateInstance(
            IDiscordClient client,
            IRole virusRole,
            IRole deadRole,
            int limit)
        {
            if (Instance != null) return Instance;

            Instance = new Infector(client, virusRole, deadRole, limit);
            return Instance;
        }
        public async Task Infect(IGuildUser user)
        {
            var members = await user.Guild.GetUsersAsync(CacheMode.AllowDownload);
            int numInfected = 
               (from member in members
                where member.RoleIds.Contains(_virusRole.Id)
                select member).Count();

            if (numInfected >= _limit) throw new LimitException(_limit, numInfected);
            await user.AddRoleAsync(_virusRole);
        }

        public async Task Disinfect(IGuildUser user)
        {
            await user.RemoveRoleAsync(_virusRole);
        }

        public async Task Kill(IGuildUser user)
        {
            await user.AddRoleAsync(_deadRole);
        }
    }
}