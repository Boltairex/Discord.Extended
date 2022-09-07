using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Discord.Extended.Models
{
    public abstract class MessageCommandBase : ApplicationCommandBase
    {
        public override void Initialize() => Command = GetCommand().Build();

        public override async Task ExecuteAsync(SocketCommandBase interaction, IUser author)
        {
            await ExecuteAsync((SocketMessageCommand)interaction, (SocketUser)author);
        }

        /// <summary>
        /// Executes command with received interaction and author.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(SocketMessageCommand interaction, SocketUser author);

        /// <summary>
        /// Get command with initialization.
        /// </summary>
        /// <returns></returns>
        protected abstract MessageCommandBuilder GetCommand();
    }
}