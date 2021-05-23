using Discord.Commands;
using System.Threading.Tasks;

namespace Discord.Extended
{
    public abstract class Extended<T> : ModuleBase<T> where T : SocketCommandContext
    {
        public ExtendedService extended { get; set; }

        /// <summary>
        /// Send PagedMessage to channel, and automatically adds all specified <see cref="InteractiveIEmote"/>.
        /// </summary>
        /// <param name="pagedMessage"></param>
        /// <returns></returns>
        public async Task SendPagedMessage(PagedMessage pagedMessage) => await extended.SendPagedMessage(pagedMessage, Context); 
    }
}
