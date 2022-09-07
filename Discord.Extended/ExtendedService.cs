using Discord.WebSocket;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.Commands;
using System.Linq;
using Discord.Rest;
using System;
using System.Timers;
using Discord.Extended.Models;

namespace Discord.Extended
{
    public class ExtendedService
    {
        public static ExtendedService Instance { get; private set; }
        readonly static object threadLock = new object();

        /// <summary> Safe from Rate-Limit module.
        /// </summary>
        public static bool smartMessages = true;
        /// <summary> Interval, after a message will be changed. Requires <see cref="smartMessages"/> set to 'true'.
        /// Value is dynamicly used.</summary>
        /// <remarks>
        /// Caution! It's not recommended to set lower values than basic value, if you plan to use library to bigger bots, or when users are very active. Also works kinda weird. <para></para> Basic value: <see langword="450"></see>.
        /// </remarks>
        public static uint milisecondsAwait = 450;

        readonly DiscordSocketClient client;

        Dictionary<ulong, SmartMessageInfo> pendingChanges { get; } = new Dictionary<ulong, SmartMessageInfo>();
        Dictionary<IMessage, PagedMessage> pagedMessages { get; } = new Dictionary<IMessage, PagedMessage>();

        System.Timers.Timer lifeTimeOfMessages;

        /// <summary>
        /// Thread-safe singleton initialization.
        /// </summary>
        /// <param name="client"></param>
        public ExtendedService(DiscordSocketClient client)
        {
            lock (threadLock)
            {
                if (Instance != null) return;
                this.client = client;
                this.client.ReactionAdded += DiscordReactionAdded;
                this.client.ReactionRemoved += DiscordReactionRemoved;
                lifeTimeOfMessages = new System.Timers.Timer();
                lifeTimeOfMessages.Elapsed += ExpiredMessagesCollector;
                Instance = this;
            }
        }

        async Task DiscordReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
            try
            {
                if (arg3.UserId == client.CurrentUser.Id) return;
                if (!pagedMessages.Any(x => x.Key.Id == arg3.MessageId)) return;

                var message = pagedMessages.Single(x => x.Key.Id == arg3.MessageId);
                if (message.Value.Owner != null && message.Value.Owner.Id != arg3.User.Value.Id) return;

                for (int i = 0; i < message.Value.Emojis.Length; i++)
                    if (arg3.Emote.Name == message.Value.Emojis[i].Emote.Name)
                    {
                        var interactiveMessage = message.Value;
                        message.Value.Emojis[i].CallEmoteAdded(ref interactiveMessage);
                        pagedMessages[message.Key] = interactiveMessage;
                    }

                if (smartMessages)
                {
                    if (pendingChanges.ContainsKey(message.Key.Id)) pendingChanges[message.Key.Id] = new SmartMessageInfo(DateTime.Now, message.Key as IUserMessage, pendingChanges[message.Key.Id].countOfChanges);
                    else
                    {
                        pendingChanges.Add(message.Key.Id, new SmartMessageInfo(DateTime.Now, message.Key as IUserMessage, 0));
                        await ChangeMessage(message.Key.Id);
                    }
                    return;
                }

                var SocketMessage = (message.Key as RestUserMessage);
                await SocketMessage.ModifyAsync(m => m.Embed = message.Value.CurrentPage);
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        async Task DiscordReactionRemoved(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3) => await DiscordReactionAdded(arg1, arg2, arg3);
        

        void FirstItemAdded()
        {
            try
            {
                TimeSpan timerInterval = pagedMessages.First().Value.ExpirationDate - DateTime.Now;
                lifeTimeOfMessages.Interval = timerInterval.TotalMilliseconds;//Added one to be sure that time will expire
                lifeTimeOfMessages.Enabled = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //Collects outdated messages (it's not outdated, it's just collecting outdated messages)
        void ExpiredMessagesCollector(object sender, ElapsedEventArgs args)
        {
            var copyPagedMessages = pagedMessages;

            //Remove outdated elements and call OnOutdated event
            foreach (KeyValuePair<IMessage, PagedMessage> oneMessage in copyPagedMessages)
                if (oneMessage.Value.ExpirationDate < DateTime.Now)
                {
                    pagedMessages.Remove(oneMessage.Key);
                    if(!(oneMessage.Value.OnMessageOutdated is null))
                        oneMessage.Value.OnMessageOutdated();
                }

            //If list doesn't have any elements, turn off timer
            if(pagedMessages.Count == 0)
            {
                lifeTimeOfMessages.Enabled = false;
                return;
            }

            //Finding next closest element to wait for
            var nextElement = pagedMessages.Min(x => x.Value.ExpirationDate);
            lifeTimeOfMessages.Interval = (pagedMessages.First().Value.ExpirationDate - DateTime.Now).TotalMilliseconds + 1;
        }

        /// <summary>
        /// Task for rate-limit safe message change.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        private async Task ChangeMessage(ulong u)
        {
            try
            {
            point:
                await Task.Delay((int)milisecondsAwait + 50);
                var info = pendingChanges.Single(x => x.Key == u).Value;

                if (info.Equals(null)) return;
                if ((DateTime.Now - info.time).TotalMilliseconds >= milisecondsAwait)
                {
                    var originalMess = pagedMessages.Single(x => x.Key.Id == u);
                    await info.message.ModifyAsync(m => m.Embed = originalMess.Value.CurrentPage);
                    pendingChanges.Remove(u);
                    return;
                }
                goto point;
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

        public async Task SendPagedMessage(PagedMessage pagedMessage, ICommandContext context)
        {
            var message = await context.Channel.SendMessageAsync(embed: pagedMessage.CurrentPage);
            pagedMessage.Activate(message);

            foreach (InteractiveIEmote emoji in pagedMessage.Emojis)
                await message.AddReactionAsync(emoji.Emote);

            pagedMessages.Add(message, pagedMessage);

            if (pagedMessages.Count == 1)
                FirstItemAdded();
        }

        struct SmartMessageInfo
        {
            public DateTime time;
            public IUserMessage message;
            public int countOfChanges;

            public SmartMessageInfo(DateTime time, IUserMessage message, int count)
            {
                this.time = time;
                this.message = message;
                countOfChanges = count + 1;
            }
        }
    }
}