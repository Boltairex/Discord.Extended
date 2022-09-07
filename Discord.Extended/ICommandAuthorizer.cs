using Discord.WebSocket;

namespace Discord.Extended
{
    /// <summary>
    /// Simply can check if user is authorized to use secured command in <see cref="ApplicationCommandService"/>.
    /// </summary>
    public interface ICommandAuthorizer
    {
        bool CheckAuthorization(SocketCommandBase interaction, IUser user);
    }
}
