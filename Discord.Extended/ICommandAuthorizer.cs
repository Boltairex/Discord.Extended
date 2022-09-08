using Discord.WebSocket;

namespace Discord.Extended
{
    /// <summary>
    /// Simply can check if user is authorized to use secured command in <see cref="ApplicationCommandService"/>.
    /// </summary>
    public interface ICommandAuthorizer
    {
        /// <summary>
        /// Return true when authorization is granted. Method called when authorization is needed.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        bool CheckAuthorization(SocketCommandBase interaction, IUser user);
    }
}
