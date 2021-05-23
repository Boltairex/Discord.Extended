using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Extended;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
            try
            {
                Console.WriteLine("Rozruch...");
                using (var Client = new DiscordSocketClient())
                {
                    await Client.LoginAsync(TokenType.Bot, "ODI2MTg5Nzg1MzczNzM2OTky.YGI3Mg.zuf9XGUENA9ZqrEUq2eoaVcpHM0");
                    await Client.StartAsync();

                    Console.WriteLine("Zalogowano pomyślnie.");

                    CommandServiceConfig Conf = new CommandServiceConfig()
                    {
                        DefaultRunMode = RunMode.Async,
                        CaseSensitiveCommands = false
                    };

                    provider = new ServiceCollection()
                        .AddSingleton(Client)
                        .AddSingleton<ExtendedService>()
                        .BuildServiceProvider();

                    service = new CommandService(Conf);
                    service.AddModulesAsync(Assembly.GetEntryAssembly(), provider).GetAwaiter();

                    Console.WriteLine("Utworzono CommandService pomyślnie.");

                    Client.MessageReceived += HandleCommandAsync;
                    Client.ReactionAdded += ReactionAdded;
                    Client.ReactionRemoved += ReactionRemoved;

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

        async Task ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }

        async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }
    }
}
