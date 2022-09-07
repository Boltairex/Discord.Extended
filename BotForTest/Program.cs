using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Extended;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord.Extended.Models;

namespace BotForTest
{
    class Program
    {
        public CommandService service { get; private set; }
        public DiscordSocketClient client { get; private set; }
        public IServiceProvider provider { get; private set; }

        static void Main()
        {
            new Program().StartBot().GetAwaiter().GetResult();
        }

        public async Task StartBot()
        {
            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.Guilds | GatewayIntents.AllUnprivileged,
                AlwaysDownloadUsers = true
            };

            CommandServiceConfig commandConfig = new CommandServiceConfig()
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            };

            try
            {
                Console.WriteLine("Rozruch...");
                using (client = new DiscordSocketClient(socketConfig))
                {
                    var r1 = client.LoginAsync(TokenType.Bot, "ID").Exception;
                    var r2 = client.StartAsync().Exception;

                    if (r1 != null)
                        Console.WriteLine(r1);
                    if (r2 != null)
                        Console.Write(r2);

                    provider = new ServiceCollection()
                        .AddSingleton(client)
                        .AddSingleton<ExtendedService>()
                        .BuildServiceProvider();

                    service = new CommandService(commandConfig);
                    service.AddModulesAsync(Assembly.GetEntryAssembly(), provider).GetAwaiter();

                    Console.WriteLine("Update application commands? T/*");
                    var key = Console.ReadKey();

                    bool update = key.Key == ConsoleKey.T;
                    Console.WriteLine("Utworzono CommandService pomyślnie.");

                    ApplicationCommandService application = new ApplicationCommandService(client);

                    Console.WriteLine("Utworzono system aplikacji.");
                    client.MessageReceived += HandleCommandAsync;

                    client.Ready += () => {
                        application.CollectSlashCommands();
                        application.CollectUserCommands();
                        application.CollectMessageCommands();
                        application.RegisterCommands(update, true);
                        return Task.CompletedTask;
                    };

                    Console.WriteLine("Bot gotowy do działania.");

                    await Task.Delay(-1);
                }
            }catch (Exception e ) { Console.WriteLine(e); }
        }

        public async Task HandleCommandAsync(SocketMessage m)
        {
            if (!(m is SocketUserMessage Message) || Message.Source != Discord.MessageSource.User) return;

            var context = new SocketCommandContext(client, Message);
            int arg = 0;

            if (Message.HasCharPrefix('>', ref arg))
            {
                var result = await service.ExecuteAsync(context, arg, provider);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await context.Channel.SendMessageAsync($"Wystąpił błąd w strukturze skryptu. Szczegóły: {result.ErrorReason}");
            }
        }
    }
}
