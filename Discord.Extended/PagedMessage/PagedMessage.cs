using Discord.Extended.Models;
using System;
using System.Collections.Generic;

namespace Discord.Extended
{
    public delegate void MessageOutdated();

    public sealed class PagedMessage : InteractiveMessage
    {
        public MessageOutdated OnMessageOutdated;

        public Embed CurrentPage { get => Pages[currentPage]; }

        public override bool OnlyAuthorCanRespond => !(Owner is null);

        public override TimeSpan LifeTime => lifetime;

        public Embed[] Pages;
        public InteractiveIEmote[] Emojis;
        public IUser Owner;

        TimeSpan lifetime;
        // + CreationTime

        int currentPage = 0;

        public PagedMessage(TimeSpan Lifetime, EmbedBuilder[] Pages, InteractiveIEmote[] Emojis, IUser Owner = null)
        {
            CreationTime = DateTime.Now;
            lifetime = Lifetime;
            List<Embed> embeds = new List<Embed>();

            for (int x = 0; x < Pages.Length; x++)
            {
                if (string.IsNullOrEmpty(Pages[x].Title))
                    Pages[x].Title = (x + 1)  + "/" + Pages.Length;

                embeds.Add(Pages[x].Build());
            }

            this.Pages = embeds.ToArray();
            this.Emojis = Emojis;
            this.Owner = Owner;
        }

        public PagedMessage(int Lifetime, Embed[] Pages, InteractiveIEmote[] Emojis, IUser Owner = null)
        {
            CreationTime = DateTime.Now;
            lifetime = new TimeSpan(0,0,Lifetime);
            this.Pages = Pages;
            this.Emojis = Emojis;
            this.Owner = Owner;
        }
  
        /// <summary>
        /// Runned after sending first message to initialize necessary data.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeout"></param>
        public override void Activate(IMessage message, DateTime timeout = new DateTime())
        {
            Message = message;
            //Timeout = timeout;
            // tutaj będzie odwołanie do naszego Buforu by go dodać
        }

        /// <summary>
        /// Activated every activity of message. Prevents deleting from buffer.
        /// </summary>
        public override void ResetTimer()
        {
            CreationTime = new DateTime();
        }

        /// <summary>
        /// Set specified Page
        /// </summary>
        /// <param name="id"></param>
        public void SetPage(int id)
        {
            if (id >= Pages.Length || id < 0)
                throw new Exception($"The index was outside the bounds of the array. Pages in message: {Pages.Length}, Your index: {id}");

            currentPage = id;
        }

        /// <summary>
        /// Progress to next Page.
        /// </summary>
        public void NextPage()
        {
            if (currentPage + 1 >= Pages.Length) return;
            currentPage++;
        }

        /// <summary>
        /// Goes back with one Page.
        /// </summary>
        public void PreviousPage() 
        {
            if (currentPage - 1 == -1) return;
            currentPage--; 
        }
    }
}