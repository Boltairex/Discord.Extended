using Discord.WebSocket;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.Commands;
using System.Linq;
using Discord.Rest;
using System;
using System.Timers;

namespace Discord.Extended
{
    public class ExtendedService
    {
        /// <summary> Safe from Rate-Limit module.
        /// </summary>
        public static bool smartMessages = true;
        /// <summary> Interval, after a message will be changed. Requires <see cref="smartMessages"/> set to 'true'.
        /// Value is dynamicly used.</summary>
        /// <remarks>
        /// Caution! It's not recommended to set lower values than basic value, if you plan to use library to bigger bots, or when users are very active. Also works kinda weird. <para></para> Basic value: <see langword="450"></see>.
        /// </remarks>
        public static uint milisecondsAwait = 450;

        DiscordSocketClient Discord { get; }
        Dictionary<ulong, SmartMessageInfo> pendingChanges { get; } = new Dictionary<ulong, SmartMessageInfo>();
        Dictionary<IMessage, PagedMessage> pagedMessages { get; } = new Dictionary<IMessage, PagedMessage>();

        System.Timers.Timer LifeTimeOfMessages;

        public ExtendedService(DiscordSocketClient discord)
        { 
            Discord = discord;
            Discord.ReactionAdded += Discord_ReactionAdded;
            discord.ReactionRemoved += Discord_ReactionRemoved;
            LifeTimeOfMessages = new System.Timers.Timer();
            LifeTimeOfMessages.Elapsed += OutdatedMessageCollector;
        }

        void FirstItemAdded()
        {
            try
            {
                TimeSpan timerInterval = pagedMessages.First().Value.ExpirationDate - DateTime.Now;
                LifeTimeOfMessages.Interval = timerInterval.TotalMilliseconds;//Added one to be sure that time will expire
                LifeTimeOfMessages.Enabled = true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //Collects outdated messages (it's not outdated, it's just collecting outdated messages)
        void OutdatedMessageCollector(object sender, ElapsedEventArgs args)
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
                LifeTimeOfMessages.Enabled = false;
                return;
            }

            //Finding next closest element to wait for
            var nextElement = pagedMessages.Min(x => x.Value.ExpirationDate);
            LifeTimeOfMessages.Interval = (pagedMessages.First().Value.ExpirationDate - DateTime.Now).TotalMilliseconds + 1;
        }

        private Task Discord_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3) => Discord_ReactionAdded(arg1, arg2, arg3);

        private async Task Discord_ReactionAdded(Cacheable<IUserMessage, ulong> arg1,
            ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            try
            {
                if (arg3.UserId == Discord.CurrentUser.Id) return;
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
                        ChangeMessage(message.Key.Id);
                    }
                    return;
                }

                var SocketMessage = (message.Key as RestUserMessage);
                await SocketMessage.ModifyAsync(m => m.Embed = message.Value.CurrentPage);
            } catch (Exception e) { Console.WriteLine(e); }
        }

        /// <summary>
        /// Task for safe from rate-limit message change.
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
            catch (Exception e){ Console.WriteLine(e); }
        }

        public async Task SendPagedMessage(PagedMessage pagedMessage, SocketCommandContext context)
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