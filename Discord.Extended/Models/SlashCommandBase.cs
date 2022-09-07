using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Discord.Extended.Models
{
    public abstract class SlashCommandBase : ApplicationCommandBase
    {
        public override void Initialize() => Command = GetCommand().Build();

        public override async Task ExecuteAsync(SocketCommandBase interaction, IUser author)
        {
            await ExecuteAsync((SocketSlashCommand)interaction, (SocketUser)author);
        }

        /// <summary>
        /// Executes command with received interaction and author.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(SocketSlashCommand interaction, SocketUser author);

        /// <summary>
        /// Get command with initialization.
        /// </summary>
        /// <returns></returns>
        protected abstract SlashCommandBuilder GetCommand();
    }
}