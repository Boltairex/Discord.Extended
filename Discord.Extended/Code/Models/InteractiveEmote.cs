namespace Discord.Extended
{
    public delegate void OnAddUp(ref PagedMessage message);

    /// <summary>
    /// Class, that is used for callback from clicking Emoji/Emote in message.
    /// </summary>
    public class InteractiveIEmote
    {
        public IEmote Emote;
        public event OnAddUp EmoteAdded;

        public void CallEmoteAdded(ref PagedMessage message) => EmoteAdded.Invoke(ref message);
    }
}
