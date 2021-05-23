using System;
using System.Collections.Generic;

namespace Discord.Extended
{
    /// <summary>
    /// <see cref="PagedMessageCreator"/> is used to create interactive messages with multiple settings, automatic clearing from the buffer and reaction system. <para></para>
    /// Needs to be passed to ExtendedService to work.
    /// </summary>
    public class PagedMessageCreator
    {
        /// <summary>
        /// Property of the User set.
        /// </summary>
        public IUser GetUser => User;

        /// <summary>
        /// Property of added pages.
        /// </summary>
        public IReadOnlyList<EmbedBuilder> GetPages => pages;
        List<EmbedBuilder> pages;

        /// <summary>
        /// Property of added emojis.
        /// </summary>
        public IReadOnlyList<InteractiveIEmote> GetEmoijs => emojis;
        List<InteractiveIEmote> emojis;

        /// <summary>
        /// Lifetime in seconds, before it is removed from the buffer. Basic time is 60 seconds.
        /// </summary>
        public int Lifetime { get; private set; }

        /// <summary>
        /// It determines whether it can be controlled by more people than just the author.
        /// </summary>
        public bool OnlyAuthorCanRespond => !(User is null);
        IUser User;

        /// <summary>
        /// This delegate is called whenever PagedMessaged is cleared by the OutdatedMessageCollector
        /// </summary>
        public MessageOutdated OnMessageOutdated;

        /// <summary>
        /// Initializes new <see cref="PagedMessageCreator"/>, that it's used in PagedMessages to work. <para></para>
        /// Adds basic User to embeds. Ignored, when <see cref="PagedMessageInfo"/> is passed directly.
        /// </summary>
        /// <param name="user"></param>
        public PagedMessageCreator(IUser User = null)
        {
            pages = new List<EmbedBuilder>();
            emojis = new List<InteractiveIEmote>();
            Lifetime = 60;
            this.User = User;
        }

        /// <summary>
        /// Initializes new <see cref="PagedMessageCreator"/>, that it's used in PagedMessages to work. <para></para>
        /// Value of <paramref name="seconds"/> set the object lifetime.
        /// </summary>
        /// <param name="seconds"></param>
        public PagedMessageCreator(int seconds, IUser User = null)
        {
            pages = new List<EmbedBuilder>();
            emojis = new List<InteractiveIEmote>();
            Lifetime = Tools.Clamp(seconds, -1, 180);
            this.User = User;
        }

        /// <summary>
        /// Set a lifetime of object. After sending the message, the time will start to run and after passing the <see cref="Lifetime"/>, it will be removed from the buffer and destroyed.<para></para>
        /// Max time: 180 seconds. <para></para>
        /// Min time: -1 (eternity, not safe for multiple messages).
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public PagedMessageCreator SetLifetime(int seconds)
        {
            Lifetime = Tools.Clamp(seconds, -1, 180);
            return this;
        }

        /// <summary>
        /// Adds page with specified name and content.
        /// </summary>
        /// <param name="pageName"></param>
        /// <param name="pageContent"></param>
        public PagedMessageCreator AddPage(string pageName, string pageContent)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(pageContent)
                 .WithTitle(pageName);

            pages.Add(embed);
            return this;
        }

        /// <summary>
        /// Adds page with auto-increment, that sets the page name.
        /// </summary>
        /// <param name="pageContent"></param>
        /// <param name="type"></param>
        public PagedMessageCreator AddPage(string pageContent)
        {
            var embed = new EmbedBuilder();
            embed.WithDescription(pageContent);

            pages.Add(embed);
            return this;
        }

        /// <summary>
        /// Adds page with earlier specified settings.
        /// </summary>
        /// <param name="page"></param>
        public PagedMessageCreator AddPage(EmbedBuilder page)
        {
            pages.Add(page);
            return this;
        }

        /// <summary>
        /// Adds multiple pages in array order.
        /// </summary>
        /// <param name="pages"></param>
        public PagedMessageCreator AddPages(EmbedBuilder[] pages)
        {
            this.pages.AddRange(pages);
            return this;
        }

        /// <summary>
        /// Adds multiple pages in list order.
        /// </summary>
        /// <param name="pages"></param>
        public PagedMessageCreator AddPages(List<EmbedBuilder> pages)
        {
            this.pages.AddRange(pages);
            return this;
        }

        /// <summary>
        /// Adds <paramref name="emote"/> to list, that activates the assigned event when clicked.
        /// </summary>
        /// <param name="emote"></param>
        /// <returns></returns>
        public PagedMessageCreator AddInteractiveIEmote(InteractiveIEmote emote)
        {
            if(emote is null)
                throw new Exception("Passed null InteractiveIEmote.");

            if (emojis.Count + 1 > 20)
                throw new Exception("Emojis count is over limit. Discord limit: 20, Emojis count: 21");

            emojis.Add(emote);
            return this;
        }

        /// <summary>
        /// Adds <paramref name="emote"/> to list, that activates the assigned event when clicked.
        /// </summary>
        /// <param name="emote"></param>
        /// <returns></returns>
        public PagedMessageCreator AddInteractiveIEmotes(InteractiveIEmote[] emotes)
        {
            if (emotes == null)
                throw new Exception("Passed null array.");

            for (int x = 0; x < emotes.Length; x++)
                if (emotes[x] == null)
                    throw new Exception("Passed null InteractiveIEmote. Index in array: " + x);

            if (emojis.Count + emotes.Length > 20)
                throw new Exception("Emojis count is over limit. Discord limit: 20, Emojis count: " + emojis.Count + emotes.Length);

            emojis.AddRange(emotes);
            return this;
        }

        /// <summary>
        /// Adds an emoji, that activates the assigned event when clicked.
        /// </summary>
        /// <param name="emoji"></param>
        /// <param name="onaddup"></param>
        /// <returns></returns>
        public PagedMessageCreator AddInteractiveIEmote(Emoji emoji, OnAddUp onaddup)
        {
            if (emoji is null)
                throw new Exception("Passed null emoji.");

            return addInteractiveEmoji(emoji,onaddup);
        }

        /// <summary>
        /// Adds an emote, that activates the assigned event when clicked.
        /// </summary>
        /// <param name="emote"></param>
        /// <param name="onaddup"></param>
        /// <returns></returns>
        public PagedMessageCreator AddInteractiveIEmote(Emote emote, OnAddUp onaddup)
        {
            if(emote == null)
                throw new Exception("Passed null emote.");

            return addInteractiveEmoji(emote, onaddup);
        }

        /// <summary>
        /// Creates new <see cref="PagedMessage"/>, that will be used in Interactive Messages system. Passed via command <see cref="Extended.SendPagedMessage"/>
        /// </summary>
        /// <returns></returns>
        public PagedMessage Build()
        {
            if (pages.Count == 0)
                throw new Exception("There is not any page set.");
            if (emojis.Count == 0)
                throw new Exception("There is not any emoji set.");

            var message = new PagedMessage(new TimeSpan(0, 0, 0, Lifetime, 0), pages.ToArray(), emojis.ToArray(), User);
            message.OnMessageOutdated += OnMessageOutdated;

            return message;
        }

        PagedMessageCreator addInteractiveEmoji(IEmote emote, OnAddUp onaddup)
        {
            if (emojis.Count + 1 > 20)
                throw new Exception("Emojis count is over limit. Discord limit: 20, Emojis count: 21");

            var interactiveEmote = new InteractiveIEmote();
            interactiveEmote.Emote = emote;
            interactiveEmote.EmoteAdded += onaddup;
            emojis.Add(interactiveEmote);

            return this;
        }
    }
}