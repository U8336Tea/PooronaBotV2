using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PooronaBot.Config;

using Discord.Commands;

namespace PooronaBot.Commands
{
    [RequireContext(ContextType.Guild)]
    public class InformationModule : ModuleBase<SocketCommandContext>
    {
        // TODO: Allow configuration to be passed in, for possible configuration file, etc.
        private IConfiguration _config = new EnvironmentConfiguration();

        [Command("listinfected")]
        [Summary("Lists everyone who is infected.")]
        public async Task ListInfectedAsync()
        {
            var infected = await Infector.Instance.ListInfected();
            var infectedNames =
                from user in infected
                select $"{user.Username}#{user.Discriminator} ({user.Nickname})";

            var message = string.Join("\n", infectedNames);
            if (infectedNames.Count() == 0){
                await ReplyAsync("Literally no one is infected.");
            } else if (message.Length > 2000) {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(message))) {
                    await Context.Channel.SendFileAsync(stream, "infected.txt");
                }
            } else {
                await ReplyAsync(message);
            }
        }
    }
}