using Discord.Extended.Models;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Discord.Extended
{
    /// <summary>
    /// Singleton object. 
    /// </summary>
    public class ApplicationCommandService : IApplicationCommandHandler
    {
        /// <summary>
        /// Instance of created singleton object.
        /// </summary>
        public static ApplicationCommandService Instance { get; private set; }

        readonly static object threadLock = new object();
        protected readonly DiscordSocketClient client;

        protected Dictionary<Type, Dictionary<string, ApplicationCommandBase>> registry;
        protected ICommandAuthorizer authorizer;

        /// <summary>
        /// Authorizer will have null value, so internal authorization is skipped.
        /// <para></para>
        /// <paramref name="autoHandle"/> is used for auto-handling events from client (slash commands, user commands and message commands).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="autoHandle"></param>
        public ApplicationCommandService(DiscordSocketClient client, bool autoHandle = true)
        {
            lock (threadLock)
            {
                if (Instance != null) return;
                this.client = client;
                registry = new Dictionary<Type, Dictionary<string, ApplicationCommandBase>>();
                Instance = this;

                if (!autoHandle) return;
                client.SlashCommandExecuted += SlashCommandUsed;
                client.UserCommandExecuted += UserCommandUsed;
                client.MessageCommandExecuted += MessageCommandUsed;
            }
        }

        /// <summary>
        /// <paramref name="autoHandle"/> is used for auto-handling events from client (slash commands, user commands and message commands).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="autoHandle"></param>
        /// <param name="authorizer"></param>
        public ApplicationCommandService(DiscordSocketClient client, ICommandAuthorizer authorizer, bool autoHandle = true)
        {
            lock (threadLock)
            {
                if (Instance != null) return;
                this.client = client;
                this.authorizer = authorizer;
                registry = new Dictionary<Type, Dictionary<string, ApplicationCommandBase>>();
                Instance = this;

                if (!autoHandle) return;
                client.SlashCommandExecuted += SlashCommandUsed;
                client.UserCommandExecuted += UserCommandUsed;
                client.MessageCommandExecuted += MessageCommandUsed;
            }
        }

        /// <summary>
        /// Console.WriteLine with color
        /// </summary>
        /// <param name="m"></param>
        /// <param name="color"></param>
        public void Printl(string m, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = color;
            Console.Write($"[{GetType().Name}] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(m + '\n');
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Uses <see cref="CollectCommandsByType{T}"/> by passing <see cref="SlashCommandBase"/> as type.
        /// </summary>
        public virtual void CollectSlashCommands(Assembly assembly = null) => CollectCommandsByType<SlashCommandBase>(assembly);

        /// <summary>
        /// Uses <see cref="CollectCommandsByType{T}"/> by passing <see cref="UserCommandBase"/> as type.
        /// </summary>
        public virtual void CollectUserCommands(Assembly assembly = null) => CollectCommandsByType<UserCommandBase>(assembly);

        /// <summary>
        /// Uses <see cref="CollectCommandsByType{T}"/> by passing <see cref="MessageCommandBase"/> as type.
        /// </summary>
        public virtual void CollectMessageCommands(Assembly assembly = null) => CollectCommandsByType<MessageCommandBase>(assembly);

        /// <summary>
        /// Collects from assembly all types that inherits from <see cref="ApplicationCommandBase"/> and given type. Results are used later in <see cref="RegisterCommands(bool, bool)"/>.
        /// <para></para>
        /// Pass <paramref name="assembly"/> if commands exists in another assembly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        public virtual void CollectCommandsByType<T>(Assembly assembly = null) where T : ApplicationCommandBase
        {
            assembly = assembly ?? Assembly.GetEntryAssembly();

            var t = typeof(T);
            
            foreach (Type type in assembly.GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t)))
            {
                var command = (ApplicationCommandBase)Activator.CreateInstance(type);

                if (command.Disable)
                    continue;

                try
                {
                    command.Initialize();
                    if (command.Command == null)
                    {
                        Printl("Command " + command.GetType().Name + " not initialized!");
                        continue;
                    }

                    if (!command.Command.Name.IsSpecified)
                        continue;

                    string nameLower = command.Command.Name.Value.ToLower();

                    if (registry.ContainsKey(t))
                    {
                        if (registry[t].ContainsKey(nameLower))
                        {
                            Printl($"Name collision detected with object: {command.GetType()}, name {command.Command.Name.Value} is already in registry.");
                            continue;
                        }

                        registry[t].Add(nameLower, command);
                    }
                    else
                    {
                        registry.Add(t, new Dictionary<string, ApplicationCommandBase>());
                        registry[t].Add(nameLower, command);
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
            }
        }

        /// <summary>
        /// Register all commands collected by <see cref="CollectCommandsByType{T}"/>. Auto unregister not used commands. Update it only when it's actually needed.
        /// <para></para>
        /// Not necessary to await for this task (Takes too long).
        /// </summary>
        /// <returns></returns>
        public virtual async Task RegisterCommands(bool updateCommands, bool withCallback)
        {
            if (!updateCommands)
                return;

            if (withCallback)
                Printl("Updating application commands...");

            var globalCommands = new List<ApplicationCommandProperties>();
            var guildCommands = new Dictionary<ulong, List<ApplicationCommandProperties>>();
            try
            {
                foreach (var type in registry)
                {
                    foreach (var command in type.Value)
                    {
                        if (!command.Value.IsGlobal)
                        {
                            foreach (var u in command.Value.GuildsID)
                            {
                                if (guildCommands.ContainsKey(u))
                                    guildCommands[u].Add(command.Value.Command);
                                else
                                    guildCommands.Add(u, new List<ApplicationCommandProperties>() { command.Value.Command });
                            }
                        }
                        else
                            globalCommands.Add(command.Value.Command);
                    }
                }

                await client.BulkOverwriteGlobalApplicationCommandsAsync(globalCommands.ToArray());

                var allGuilds = client.Guilds.ToList();
                foreach (var kv in guildCommands)
                {
                    var guild = allGuilds.First(g => g.Id == kv.Key);

                    if (guild == null)
                    {
                        Printl("Failed to get guild with id: " + kv.Key);
                        continue;
                    }

                    allGuilds.Remove(guild);
                    await guild.BulkOverwriteApplicationCommandAsync(kv.Value.ToArray());
                }

                if (allGuilds.Count == 0) return;

                var emptyProperties = new ApplicationCommandProperties[] { };

                foreach (SocketGuild guild in allGuilds)
                {
                    if (guild.GetApplicationCommandsAsync().Result.Count > 0)
                        await guild.BulkOverwriteApplicationCommandAsync(emptyProperties);
                }
                Printl("Done.");
            }
            catch(Exception e) { Console.WriteLine(e); }
        }

        /// <summary>
        /// Trying to get registered commands category by given type category.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commands"></param>
        /// <returns></returns>
        public virtual bool TryGetCommands<T>(out IReadOnlyDictionary<string, ApplicationCommandBase> commands) where T : ApplicationCommandBase
        {
            if (registry.ContainsKey(typeof(T)))
            {
                commands = registry[typeof(T)];
                return true;
            }
            commands = null;
            return false;
        }

        /// <summary>
        /// Trying to get registered command by given name and type category.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual bool TryGetCommand<T>(string name, out ApplicationCommandBase command)
        {
            var type = typeof(T);

            if (registry.ContainsKey(type))
                if (registry[type].ContainsKey(name))
                {
                    command = registry[type][name];
                    return true;
                }
            command = null;
            return false;
        }

        /// <summary>
        /// Simply executes command when usage detected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="interaction"></param>
        /// <returns></returns>
        public virtual async Task ExecuteApplicationCommand<T>(string name, SocketCommandBase interaction)
        {
            if (interaction is null || !interaction.IsValidToken) return;

            var user = interaction.User;

            name = name.ToLower();
            if (TryGetCommand<T>(name, out var command))
            {
                Console.WriteLine(command.Command.Name.Value);
                if (command.RequireAuthorization && authorizer != null && !authorizer.CheckAuthorization(interaction, user))
                {
                    await interaction.RespondAsync(text: "No authorization granted.", ephemeral: true);
                    return;
                }

                await command.ExecuteAsync(interaction, user);
                return;
            }
            await interaction.RespondAsync(text: "Command does not exist.", ephemeral: true);
        }

        /// <summary>
        /// Rehandles interaction requested by commands.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="interaction"></param>
        /// <param name="author"></param>
        /// <returns></returns>
        public virtual async Task ReHandleInteraction<T>(string name, SocketCommandBase interaction, SocketUser author)
        {
            if (interaction is null) return;

            await ExecuteApplicationCommand<T>(name, interaction);
        }

        protected async Task MessageCommandUsed(SocketMessageCommand arg)
        {
            await ExecuteApplicationCommand<MessageCommandBase>(arg.CommandName, arg);
        }

        protected async Task UserCommandUsed(SocketUserCommand arg)
        {
            await ExecuteApplicationCommand<UserCommandBase>(arg.CommandName, arg);
        }

        protected async Task SlashCommandUsed(SocketSlashCommand arg)
        {
            await ExecuteApplicationCommand<SlashCommandBase>(arg.CommandName, arg);
        }
    }
}