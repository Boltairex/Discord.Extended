using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Discord.Extended.Models
{
    public abstract class UserCommandBase : ApplicationCommandBase
    {
        public override void Initialize() => Command = GetCommand().Build();

        public override async Task ExecuteAsync(SocketCommandBase interaction, IUser author)
        {
            await ExecuteAsync((SocketUserCommand)interaction, author);
        }

        /// <summary>
        /// Executes command with received interaction and author.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(SocketUserCommand interaction, SocketUser author);

        /// <summary>
        /// Get command with initialization.
        /// </summary>
        /// <returns></returns>
        protected abstract UserCommandBuilder GetCommand();
    }
}
