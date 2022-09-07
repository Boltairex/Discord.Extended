using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Extended.Models
{
    public abstract class ApplicationCommandBase
    {
        static IApplicationCommandHandler currentHandler;

        static ApplicationCommandBase()
        {
            if (currentHandler == null)
            {
                if (ApplicationCommandService.Instance == null)
                    Console.WriteLine($"Handler is inactive. {nameof(ReHandleInteraction)} method is disabled. Use {nameof(SetHandler)} method, or early initialize {nameof(ApplicationCommandService)}.");
                else
                    currentHandler = ApplicationCommandService.Instance;
            }
        }

        /// <summary>
        /// Required if using <see cref="ReHandleInteraction{T}(string, SocketCommandBase, SocketUser)"/>, and want to use your own handler
        /// </summary>
        /// <param name="handler"></param>
        public static void SetHandler(IApplicationCommandHandler handler)
        {
            currentHandler = handler;
        }

        /// <summary>
        /// Checks if command is global.
        /// </summary>
        public bool IsGlobal => GuildsID == null || GuildsID.Length == 0;

        /// <summary>
        /// Builded command after initialization.
        /// </summary>
        public ApplicationCommandProperties Command { get; protected set; }

        /// <summary>
        /// If we want to disable our command from loading to memory or updating.
        /// </summary>
        public virtual bool Disable => false;

        /// <summary>
        /// Checks if command need internal authorization (not Discord permissions).
        /// </summary>
        public virtual bool RequireAuthorization => false;

        /// <summary>
        /// Checks is command visible only to sender.
        /// </summary>
        public virtual bool IsEphmeral { get; } = false;

        /// <summary>
        /// If set to another value than null, command will be loaded only for specified guilds.
        /// </summary>
        public virtual ulong[] GuildsID { get; } = { };

        /// <summary>
        /// External command authorization (Discord permissions). Not used in <see cref="ApplicationCommandService"/>.
        /// </summary>
        public virtual GuildPermission Permission { get; }

        public virtual async Task ReHandleInteraction<T>(string name, SocketCommandBase interaction, SocketUser author)
        {
            if (currentHandler is null)
                throw new System.Exception($"Handler does not exists. Use {nameof(SetHandler)} function first.");
            await currentHandler.ReHandleInteraction<T>(name, interaction, author);
        }

        /// <summary>
        /// Initialize command. It must set <see cref="Command"/> field.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Executes command with received interaction and author.
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(SocketCommandBase interaction, IUser author);
    }
}