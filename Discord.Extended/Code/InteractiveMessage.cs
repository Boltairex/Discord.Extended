using System;

namespace Discord.Extended
{
    public abstract class InteractiveMessage
    {
        public DateTime CreationTime;
        public abstract TimeSpan LifeTime { get; }
        public DateTime ExpirationDate { get => CreationTime + LifeTime; }

        public IMessage Message;

        public abstract bool OnlyAuthorCanRespond { get; }

        public abstract void Activate(IMessage message, DateTime timeout);
        public abstract void ResetTimer();
    }
}
