using System;

namespace Discord.Extended.Applications
{
    /// <summary>
    /// Stores information about collected <see cref="IMessageComponent"/> and it's row by <see cref="XmlToComponents"/>.
    /// </summary>
    public struct CollectedIComponent : IComparable<uint>, IComparable<int>, IComparable
    {
        /// <summary>
        /// The component's row. If null - automatically set in specified order.
        /// </summary>
        public uint? Row { get; set; }

        /// <inheritdoc cref="IMessageComponent"/>
        public IMessageComponent Component { get; }

        public CollectedIComponent(int row, IMessageComponent component)
        {
            if (row < 0)
                Row = null;
            else
                Row = (uint)row;

            Component = component;
        }

        public int CompareTo(uint other)
        {
            return (int)Row.Value - (int)other;
        }

        public int CompareTo(int other)
        {
            return (int)Row.Value - other;
        }

        public int CompareTo(object obj)
        {
            if (obj is uint ui)
                return CompareTo(ui);
            else if (obj is int i)
                return CompareTo(i);
            return 0;
        }
    }
}