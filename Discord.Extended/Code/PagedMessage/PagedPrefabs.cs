namespace Discord.Extended
{
    public static class PagedPrefabs
    {
        /// <summary>
        /// Contains "◀️"
        /// </summary>
        public static Emoji LeftArrow { get; } = new Emoji("◀️");
        /// <summary>
        /// Contains "▶️"
        /// </summary>
        public static Emoji RightArrow { get; } = new Emoji("▶️");
        /// <summary>
        /// Contains "⏮️"
        /// </summary>
        public static Emoji SkipToStart { get; } = new Emoji("⏮️");
        /// <summary>
        /// Contains "⏭️"
        /// </summary>
        public static Emoji SkipToEnd { get; } = new Emoji("⏭️");


        /// <summary>
        /// Contains ready set of PreviousPage() and NextPage() event.
        /// </summary>
        /// <returns></returns>
        public static InteractiveIEmote[] BasicArrowModel()
        {
            var LA = new InteractiveIEmote();
            LA.Emote = LeftArrow;
            LA.EmoteAdded += PagedEvents.PreviousPage;

            var RA = new InteractiveIEmote();
            RA.Emote = RightArrow;
            RA.EmoteAdded += PagedEvents.NextPage;

            return new InteractiveIEmote[2] { LA, RA };
        }

        /// <summary>
        /// Contains ready set of FirstPage(), PreviousPage(), NextPage() and LastPage() event.
        /// </summary>
        /// <returns></returns>
        public static InteractiveIEmote[] FullSetModel()
        {
            var STS = new InteractiveIEmote();
            STS.Emote = SkipToStart;
            STS.EmoteAdded += PagedEvents.FirstPage;

            var LA = new InteractiveIEmote();
            LA.Emote = LeftArrow;
            LA.EmoteAdded += PagedEvents.PreviousPage;

            var RA = new InteractiveIEmote();
            RA.Emote = RightArrow;
            RA.EmoteAdded += PagedEvents.NextPage;

            var STE = new InteractiveIEmote();
            STE.Emote = SkipToEnd;
            STE.EmoteAdded += PagedEvents.LastPage;

            return new InteractiveIEmote[4] { STS, LA, RA, STE };
        }

        /// <summary>
        /// Contains ready set of NextPage() and LastPage() event.
        /// </summary>
        /// <returns></returns>
        public static InteractiveIEmote[] StoryTellingModel()
        {
            var RA = new InteractiveIEmote();
            RA.Emote = RightArrow;
            RA.EmoteAdded += PagedEvents.NextPage;

            var STE = new InteractiveIEmote();
            STE.Emote = SkipToEnd;
            STE.EmoteAdded += PagedEvents.LastPage;

            return new InteractiveIEmote[2] { RA, STE };
        }
    }
}