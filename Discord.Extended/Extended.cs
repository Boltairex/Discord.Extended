using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Discord.Extended
{
    public abstract class Extended<T> : ModuleBase<T> where T : class, ICommandContext
    {
        public new T Context { get => base.Context; }

        public ExtendedService extended { get; set; }

        CommandInfo info;

        protected override void BeforeExecute(CommandInfo command)
        {
            info = command;
            base.BeforeExecute(command);
        }

        public virtual void Print(string m, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = color;
            Console.Write($"[{GetType().Name}::{info.Name}] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(m + '\n');
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public virtual void Print(Exception m, ConsoleColor color = ConsoleColor.Cyan) => Print(m.Message, color);

        public async Task SendTemporaryMessage(string message, int timeout = 3000)
        {
            try
            {
                var m = await Context.Channel.SendMessageAsync(message);
                await Task.Delay(timeout);
                await m.DeleteAsync();
            }
            catch { }
        }

        /// <summary>
        /// Safe message delete without error.
        /// </summary>
        /// <returns></returns>
        public async Task DeleteMessage()
        {
            try
            {
                await Context.Message.DeleteAsync();
            }
            catch { }
        }

        /// <summary>
        /// Send PagedMessage to channel, and automatically adds all specified <see cref="InteractiveIEmote"/>.
        /// </summary>
        /// <param name="pagedMessage"></param>
        /// <returns></returns>
        public async Task SendPagedMessage(PagedMessage pagedMessage) => await extended.SendPagedMessage(pagedMessage, Context);
    }
}