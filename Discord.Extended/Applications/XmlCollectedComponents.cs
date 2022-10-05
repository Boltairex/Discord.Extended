using System;
using System.Collections;
using System.Collections.Generic;

namespace Discord.Extended.Applications
{
    /// <summary>
    /// Class for storing components from <see cref="XmlToComponents"/>
    /// </summary>
    public class XmlCollectedComponents : IEnumerator, IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Name of current component.
        /// </summary>
        public string CustomId => kv.Key;

        /// <summary>
        /// Currently pointed object in collection.
        /// </summary>
        public object Current => kv.Value;

        /// <inheritdoc cref="Current"/>
        public Modal CurrentAsModal => Current as Modal;

        /// <inheritdoc cref="Current"/>
        public List<CollectedIComponent> CurrentAsComponents => Current as List<CollectedIComponent>;

        /// <inheritdoc cref="Current"/>
        public MessageComponent CurrentAsMessageComponent => Current as MessageComponent;

        /// <inheritdoc cref="Current"/>
        public EmbedBuilder CurrentAsEmbedBuilder => Current as EmbedBuilder;

        /// <summary>
        /// Checks if <see cref="Current"/> is <see cref="Modal"/>
        /// </summary>
        public bool IsModal => CheckCurrentType<Modal>();

        /// <summary>
        /// Checks if <see cref="Current"/> is <see cref="List{CollectedIComponent}"/> with T <see cref="CollectedIComponent"/>.
        /// </summary>
        public bool IsComponentCollection => CheckCurrentType<List<CollectedIComponent>>();

        /// <summary>
        /// Checks if <see cref="Current"/> is <see cref="MessageComponent"/>.
        /// </summary>
        public bool IsMessageComponent => CheckCurrentType<MessageComponent>();

        /// <summary>
        /// Checks if <see cref="Current"/> is <see cref="EmbedBuilder"/> (not <see cref="Embed"/>, because it's not possible to change it's values).
        /// </summary>
        public bool IsEmbedBuilder => CheckCurrentType<EmbedBuilder>();

        bool CheckCurrentType<T>() => Current.GetType() == typeof(T);

        KeyValuePair<string, object> kv;
        IEnumerator<KeyValuePair<string, object>> enumerator;

        readonly IReadOnlyDictionary<string, object> collectedObjects;

        internal XmlCollectedComponents(Dictionary<string, object> collectedObjects)
        {
            this.collectedObjects = collectedObjects;
            enumerator = this.collectedObjects.GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerator.MoveNext()"/>
        public bool MoveNext()
        {
            if (enumerator.MoveNext())
            {
                kv = enumerator.Current;
                return true;
            }
            return false;
        }

        /// <inheritdoc cref="IEnumerator.Reset()"/>
        public void Reset()
        {
            enumerator.Reset();
        }

        /// <summary>
        /// Gets created enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return enumerator;
        }

        /// <inheritdoc cref="GetEnumerator()"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return enumerator;
        }
    }
}