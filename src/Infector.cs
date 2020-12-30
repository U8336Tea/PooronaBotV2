using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using PooronaBot.Exceptions;

using Discord;

namespace PooronaBot
{
    class Infector
    {
        // TODO: There's probably a better way to do this.
        public static Infector Instance {get; private set;}
        
        private IGuild _guild;
        private IRole _virusRole;
        private IRole _deadRole;
        private List<ulong> _susceptibleRoleIDs;
        private int _limit;
        private Random _random = new Random();

        private Infector(
            IGuild guild,
            IRole virusRole,
            IRole deadRole,
            List<ulong> susceptibleRoleIDs,
            int limit)
        {
            _guild = guild;
            _virusRole = virusRole;
            _deadRole = deadRole;
            _susceptibleRoleIDs = susceptibleRoleIDs;
            _limit = limit;
        }

        public Infector CreateInstance(
            IGuild guild,
            IRole virusRole,
            IRole deadRole,
            List<ulong> susceptibleRoleIDs,
            int limit)
        {
            if (Instance != null) return Instance;

            Instance = new Infector(guild, virusRole, deadRole, susceptibleRoleIDs, limit);
            return Instance;
        }
        public async Task Infect(IGuildUser user)
        {
            var members = await _guild.GetUsersAsync(CacheMode.AllowDownload);
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

        public async Task InfectRandom()
        {
            var members = await _guild.GetUsersAsync(CacheMode.AllowDownload);
            
            var eligible = 
                from member in members
                where member.RoleIds.Intersect(_susceptibleRoleIDs).Count() > 0
                select member;

            var eligibleArray = eligible.ToArray();
            var randomUser = eligibleArray[_random.Next(eligibleArray.Count())];
            await Infect(randomUser);
        }
    }
}