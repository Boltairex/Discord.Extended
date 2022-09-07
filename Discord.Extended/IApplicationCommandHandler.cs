using Discord.WebSocket;
using System.Threading.Tasks;

namespace Discord.Extended
{
    public interface IApplicationCommandHandler
    {
        Task ReHandleInteraction<T>(string name, SocketCommandBase interaction, SocketUser author);
        Task ExecuteApplicationCommand<T>(string name, SocketCommandBase interaction);
    }
}