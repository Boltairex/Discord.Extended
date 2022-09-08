using Discord.WebSocket;
using System.Threading.Tasks;

namespace Discord.Extended
{
    public interface IApplicationCommandHandler
    {       
        /// <summary>
        /// Rehandles interaction requested by commands.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="interaction"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        Task ReHandleInteraction<T>(string name, SocketCommandBase interaction, SocketUser author);

        /// <summary>
        /// Simply executes command when usage detected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="interaction"></param>
        /// <returns></returns>
        Task ExecuteApplicationCommand<T>(string name, SocketCommandBase interaction);
    }
}