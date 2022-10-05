using System;
using System.Xml;
using Discord.Extended;

namespace Discord.Extended.Applications
{
    internal class XmlStructureException : Exception
    {
        public XmlStructureException(string message, XmlReader obj, string location) : base(message)
        {
            string attribute = "don't have one.";
            if(obj.AttributeCount > 0)
                attribute = obj.GetAttribute(0);

            Tools.PrintWarning("Additional info:" +
            $"\nError location: '{location}', node type: '{obj.NodeType}', name: '{obj.Name}'" +
            $"\nReader depth: {obj.Depth}, first attribute: {attribute}", "[!]");
        }

        public XmlStructureException(string message, string location) : base(message)
        {
            Tools.PrintWarning(
                $"\nError location: {location}.", "[!]");
        }
    }
}
