using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using Newtonsoft.Json;

namespace Discord.Extended.Applications
{
    /// <summary>
    /// Static class for loading XML data and interpret it to similar types of <see cref="IMessageComponent"/>.
    /// </summary>
    public static class XmlToComponents
    {
        #region Keys
        const string k_description = "description";
        const string k_customId = "customId";
        const string k_modal = "modal";
        const string k_components = "components";
        const string k_collection = "collection";
        const string k_value = "value";
        const string k_title = "title";
        const string k_buttons = "buttons";
        const string k_button = "button";
        const string k_options = "options";
        const string k_option = "option";
        const string k_textArea = "textArea";
        const string k_row = "row";
        const string k_label = "label";
        const string k_isDefault = "isDefault";
        const string k_isRequired = "isRequired";
        const string k_isDisabled = "isDisabled";
        const string k_placeholder = "placeholder";
        const string k_emote = "emote";
        const string k_style = "style";
        const string k_maxLength = "maxLength";
        const string k_minLength = "minLength";
        const string k_maxValues = "maxValues";
        const string k_minValues = "minValues";
        const string k_embed = "embed";
        const string k_url = "url";
        const string k_isInline = "isInline";
        const string k_field = "field";
        const string k_thumbnailUrl = "thumbnailUrl";
        const string k_imageUrl = "imageUrl";
        const string k_color = "color";
        const string k_timestamp = "timestamp";
        const string k_footer = "footer";
        const string k_fieldgroup = "fieldgroup";
        #endregion

        static string filename;

        /// <summary>
        /// Returns as single string all used keywords.
        /// </summary>
        /// <returns></returns>
        public static string GetUsedKeywords()
        {
            var fields = typeof(XmlToComponents).GetFields();
            return string.Join(", ", fields.Select(field =>
                    field.Name.StartsWith("k_") &&
                    field.Attributes.HasFlag(System.Reflection.FieldAttributes.Literal)));
        }

        /// <summary>
        /// Loads XML from file and returns objects by ascending order if needed (row), wrapped in IEnumerator.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static XmlCollectedComponents CreateComponentsFromXMLFile(string path)
        {
            using (var reader = new StreamReader(path))
            {
                filename = Path.GetFileName(path);
                return CollectStructures(reader.BaseStream);
            }
            throw new Exception("bruh");
        }

        /// <summary>
        /// Loads XML from memory and returns objects by ascending order if needed (row), wrapped in IEnumerator.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static XmlCollectedComponents CreateComponentsFromXML(string content)
        {
            using (var memory = new MemoryStream())
            {
                filename = "MemoryStream: " + memory.GetHashCode();
                memory.Write(Encoding.UTF8.GetBytes(content));
                memory.Position = 0;
                return CollectStructures(memory);
            }
            throw new Exception("bruh");
        }

        static XmlCollectedComponents CollectStructures(Stream stream)
        {
            var collectedObjects = new Dictionary<string, object>();
            var r = XmlReader.Create(stream);

            while (r.Read())
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case k_modal:
                        var modal = XmlCollectModal(ref r, r.Depth);

                        if (collectedObjects.ContainsKey(modal.CustomId))
                            throw new Exception($"Name collision! {modal.CustomId} {k_customId} is repeating in registry.");

                        collectedObjects.Add(modal.CustomId, modal.Build());
                        break;

                    case k_components:
                        if(!r.TryGetAttribute(k_customId, out var keyComponents))
                            throw new XmlStructureException($"{k_customId} attribute is necessary!", r, filename);

                        var component = XmlCollectMessageComponent(ref r, r.Depth);

                        if (collectedObjects.ContainsKey(keyComponents))
                            throw new Exception($"Name collision! {keyComponents} {k_customId} is repeating.");

                        collectedObjects.Add(keyComponents, component);
                        break;

                    case k_collection:
                        if(!r.TryGetAttribute(k_customId, out var keyCollection))
                            throw new XmlStructureException($"{k_customId} attribute is necessary!", r, filename);

                        var collection = XmlCollectComponents(ref r, r.Depth);

                        if (collectedObjects.ContainsKey(keyCollection))
                            throw new Exception($"Name collision! {keyCollection} {k_customId} is repeating in registry.");

                        collectedObjects.Add(keyCollection, collection);
                        break;

                    case k_embed:
                        var embedBuilder = XmlCollectEmbedBuilder(ref r, r.Depth);
                        
                        if (collectedObjects.ContainsKey(embedBuilder.Title))
                            throw new Exception($"Name collision! {embedBuilder.Title} {k_title} is repeating in registry.");

                        collectedObjects.Add(embedBuilder.Title, embedBuilder);
                        break;
                }
            }

            return new XmlCollectedComponents(collectedObjects);
        }

        static MessageComponent XmlCollectMessageComponent(ref XmlReader r, int depth)
        {
            ComponentBuilder builder = new ComponentBuilder();

            var list = XmlCollectComponents(ref r, depth);

            ActionRowBuilder b = new ActionRowBuilder();
            uint curRow = list.First().Row.Value;
            foreach (var l in list)
            {
                if (curRow != l.Row.Value)
                {
                    if (b.Components.Count > 0)
                    {
                        builder.AddRow(b);
                        b = new ActionRowBuilder();
                    }
                    curRow = l.Row.Value;
                }
                b.AddComponent(l.Component);
            }
            builder.AddRow(b);

            return builder.Build();
        }

        static ModalBuilder XmlCollectModal(ref XmlReader r, int depth)
        {
            var modal = new ModalBuilder();

            if (r.TryGetAttribute(k_customId, out string customId))
                modal.WithCustomId(customId);

            if (r.TryGetAttribute(k_title, out string title))
                modal.WithTitle(title);
            else
                modal.WithTitle(modal.CustomId);

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case k_textArea:
                        var text = GetTextArea(ref r, r.Depth);
                        modal.AddTextInput(text);
                        break;
                }
            }

            return modal;
        }

        static List<CollectedIComponent> XmlCollectComponents(ref XmlReader r, int depth)
        {
            var collectedComponents = new List<CollectedIComponent>();
            var unsorted = new List<CollectedIComponent>();

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case k_options:
                        if (!r.HasAttributes)
                            continue;
                        var (select, rowNumber) = GetSelectMenu(ref r, r.Depth);
                        AddToList(new CollectedIComponent(rowNumber, select.Build()));
                        break;

                    case k_buttons:
                        var (buttons, rowNumber2) = GetButtons(ref r, r.Depth);
                        foreach (var button in buttons)
                            AddToList(new CollectedIComponent(rowNumber2, button.Build()));
                        break;
                }
            }

            uint row = collectedComponents.Count > 0 ? collectedComponents.Last().Row.Value + 1 : 0;
            for (uint x = 0; x < unsorted.Count; x++)
            {
                var component = unsorted[(int)x];
                component.Row = row;
                row++;
                collectedComponents.Add(component);
            }

            unsorted.Clear();

            return collectedComponents;

            //methods
            void AddToList(CollectedIComponent component)
            {
                if (!component.Row.HasValue)
                {
                    unsorted.Add(component);
                    return;
                }

                // ordered-insert
                for (int x = 0; x < collectedComponents.Count; x++)
                {
                    if (collectedComponents[x].Row.Value > component.Row.Value)
                    {
                        collectedComponents.Insert(x, component);
                        return;
                    }
                }
                collectedComponents.Add(component);
            }
        }

        /// <param name="r"></param>
        /// <param name="depth"></param>
        /// <returns>List of ButtonBuilder and row.</returns>
        /// <exception cref="XmlStructureException"></exception>
        static (List<ButtonBuilder>, int) GetButtons(ref XmlReader r, int depth)
        {
            List<ButtonBuilder> buttons = new List<ButtonBuilder>();
            int rowNumber = -1;
            if (r.TryGetAttribute(k_row, out string row))
                int.TryParse(row, out rowNumber);

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element || r.Name != k_button || r.AttributeCount == 0)
                    continue;

                var builder = new ButtonBuilder();

                // Url and idKeyword cannot exist at the same time (atleast in builder)
                string _url;
                string _id;
                IEmote _emote = null;

                if (r.TryGetAttribute(k_emote, out string emote))
                {
                    if (Emote.TryParse("<"+emote+">", out var e))
                        _emote = e;
                    else if (Emoji.TryParse("<"+emote+">", out var em))
                        _emote = em;
                }

                r.TryGetAttribute(k_url, out _url);
                r.TryGetAttribute(k_customId, out _id);

                if (_id == null)
                    throw new XmlStructureException($"{k_customId} attribute is necessary! 'button' node. It's necessary even if 'url' attribute is given, because it replaces the 'label'.", r, filename);
                else if (_url != null)
                    builder.WithUrl(_url);
                else
                    builder.WithCustomId(_id);

                if (r.TryGetAttribute(k_label, out string label))
                    builder.WithLabel(label);
                else if (_emote == null)
                    builder.WithLabel(_id);
                else
                    builder.WithLabel(" ");

                if (r.TryGetAttribute(k_style, out string style))
                    builder.WithStyle(style.ToButtonStyle());
                else
                    builder.WithStyle(ButtonStyle.Primary);

                if (r.TryGetAttribute(k_isDisabled, out var disabled))
                    builder.WithDisabled(disabled.ToBool());

                if (_emote != null)
                    builder.WithEmote(_emote);

                buttons.Add(builder);
            }
            return (buttons, rowNumber);
        }

        static TextInputBuilder GetTextArea(ref XmlReader r, int depth)
        {
            var builder = new TextInputBuilder();

            if (r.TryGetAttribute(k_customId, out var customId))
                builder.WithCustomId(customId);
            else
                throw new XmlStructureException($"{k_customId} attribute is necessary!", r, filename);

            if (r.TryGetAttribute(k_label, out var label))
                builder.WithLabel(label);

            if (r.TryGetAttribute(k_minLength, out var minLength))
                builder.WithMinLength(int.Parse(minLength));

            if (r.TryGetAttribute(k_maxLength, out var maxLength))
                builder.WithMaxLength(int.Parse(maxLength));

            if (r.TryGetAttribute(k_style, out var style))
                builder.WithStyle(ToTextStyle(style));

            if (r.TryGetAttribute(k_value, out var value))
                builder.WithValue(value);

            if (r.TryGetAttribute(k_placeholder, out var placeholder))
                builder.WithPlaceholder(placeholder);
            else
                builder.WithPlaceholder("...");

            if (r.TryGetAttribute(k_isRequired, out var isRequired))
                builder.WithRequired(ToBool(isRequired));

            return builder;
        }

        /// <param name="r"></param>
        /// <param name="depth"></param>
        /// <returns>SelectMenuBuilder and row.</returns>
        /// <exception cref="XmlStructureException"></exception>
        static (SelectMenuBuilder, int) GetSelectMenu(ref XmlReader r, int depth)
        {
            var builder = new SelectMenuBuilder();
            int rowNumber = -1;
            if (r.TryGetAttribute(k_row, out string row))
                int.TryParse(row, out rowNumber);

            if (r.TryGetAttribute(k_customId, out string customId))
                builder.WithCustomId(customId);
            else
                throw new XmlStructureException($"{k_customId} attribute is necessary!", r, filename);

            if (r.TryGetAttribute(k_minValues, out string minValues))
                builder.WithMinValues(int.Parse(minValues));

            if (r.TryGetAttribute(k_maxValues, out string maxValues))
                builder.WithMaxValues(int.Parse(maxValues));

            if (r.TryGetAttribute(k_isDisabled, out string disabled))
                builder.WithDisabled(disabled.ToBool());

            if (r.TryGetAttribute(k_placeholder, out string placeholder))
                builder.WithPlaceholder(placeholder);
            else
                builder.WithPlaceholder("...");

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element || r.Name != k_option)
                    continue;

                if (r.AttributeCount > 0)
                {
                    string val;
                    string lab;
                    string desc;
                    bool? def = null;

                    if (!r.TryGetAttribute(k_value, out val))
                        throw new XmlStructureException($"{k_value} attribute is necessary!", r, filename);

                    if (!r.TryGetAttribute(k_label, out lab))
                        lab = val;

                    r.TryGetAttribute(k_description, out desc);

                    if (r.TryGetAttribute(k_isDefault, out string isDefaultVal))
                        def = isDefaultVal.ToBool();

                    builder.AddOption(lab, val, desc, null, def);
                }
            }
            return (builder, rowNumber);
        }

        static EmbedBuilder XmlCollectEmbedBuilder(ref XmlReader r, int depth)
        {
            EmbedBuilder builder = new EmbedBuilder();

            if (r.TryGetAttribute(k_title, out var title))
                builder.WithTitle(title);
            else
                throw new XmlStructureException($"{k_title} attribute is necessary!", r, filename);

            if (r.TryGetAttribute(k_color, out var color))
                builder.WithColor(color.ToColor());

            if (r.TryGetAttribute(k_description, out var description))
                builder.WithDescription(description);

            if (r.TryGetAttribute(k_timestamp, out var timestamp))
                builder.WithTimestamp(timestamp.ToTimestamp());

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case k_field:
                        builder.AddField(GetEmbedField(ref r, r.Depth));
                        break;

                    case k_fieldgroup:
                        builder.WithFields(GetEmbedGroup(ref r, r.Depth));
                        break;

                    case k_url:
                        if (r.TryGetUrl(out var url))
                            builder.WithUrl(url);
                        break;

                    case k_thumbnailUrl:
                        if (r.TryGetUrl(out var thumbnail))
                            builder.WithThumbnailUrl(thumbnail);
                        break;

                    case k_imageUrl:
                        if (r.TryGetUrl(out var image))
                            builder.WithImageUrl(image);
                        break;

                    case k_footer:
                        builder.WithFooter(GetEmbedFooter(ref r, r.Depth));
                        break;
                }
            }
            return builder;
        }

        static List<EmbedFieldBuilder> GetEmbedGroup(ref XmlReader r, int depth)
        {
            List<EmbedFieldBuilder> builders = new List<EmbedFieldBuilder>();

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch(r.Name)
                {
                    case k_field:
                        builders.Add(GetEmbedField(ref r, depth));
                        break;

                    default:
                        throw new XmlStructureException("Unrecognized node type!", r, filename);
                }
            }

            for(int x = 0; x < builders.Count(); x++)
                builders[x].WithIsInline(true);

            return builders;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <exception cref="XmlStructureException"></exception>
        static EmbedFieldBuilder GetEmbedField(ref XmlReader r, int depth)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();

            if (r.TryGetAttribute(k_label, out string name))
                builder.WithName(name);
            else
                throw new XmlStructureException($"{k_label} attribute is necessary!", r, filename);

            if (r.TryGetContent(out var value))
                builder.WithValue(value);
            else
                throw new XmlStructureException($"{nameof(EmbedField)} value is null, add text between nodes like this:" +
                    $"\n<field> Some text </field>", r, filename);

            if (r.TryGetAttribute(k_isInline, out string inline))
                builder.WithIsInline(inline.ToBool());

            return builder;
        }

        static EmbedFooterBuilder GetEmbedFooter(ref XmlReader r, int depth)
        {
            EmbedFooterBuilder builder = new EmbedFooterBuilder();

            while (r.Read() && depth < r.Depth)
            {
                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case k_url:
                        if (r.TryGetUrl(out var url))
                            builder.WithIconUrl(url);
                        break;

                    case k_value:
                        if (r.TryGetContent(out var content))
                            builder.WithText(content);
                        break;
                }
            }

            if (builder.IconUrl == null && builder.Text == null)
                throw new XmlStructureException($"{nameof(EmbedFooterBuilder)} is empty!", r, filename);

            return builder;
        }
    
        //Extensions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        static bool TryGetUrl(this XmlReader r, out string url)
        {
            if (r.TryGetContent(out url))
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="keyword"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static bool TryGetAttribute(this XmlReader r, string keyword, out string value)
        {
            if (r.GetAttribute(keyword) is string val && !string.IsNullOrEmpty(val))
            {
                value = val;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Gets content of node if exists.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="depth"></param>
        /// <param name="content"></param>
        /// <returns>false and out <paramref name="content"/> with value "" if content don't exist.</returns>
        static bool TryGetContent(this XmlReader r, out string content, int depth = -1)
        {
            if (depth == -1)
                depth = r.Depth;

            content = "";
            while (r.Read() && depth < r.Depth)
            {
                if (r.HasValue && r.NodeType == XmlNodeType.Text)
                {
                    content = r.Value;
                    return true;
                }
                break;
            }
            return !string.IsNullOrEmpty(content);
        }

        static DateTimeOffset ToTimestamp(this string dateText) 
        {
            if (dateText == "now")
                return DateTimeOffset.Now;

            bool fromNow = false;
            var end = dateText.Substring(dateText.Length - 2);

            if (end.ToLower() == "-n") {
                fromNow = true;
                dateText = dateText.Substring(0, dateText.Length - 2);
            }

            var results = dateText.Split(':');
            int index = results.Length;
            int year, month, day, hour, minute, second;

            second = ParseToTheLeft() + (fromNow ? DateTime.UtcNow.Second : 0);
            minute = ParseToTheLeft() + (fromNow ? DateTime.UtcNow.Minute : 0);
            hour = ParseToTheLeft() + (fromNow ? DateTime.UtcNow.Hour : 0);
            day = ParseToTheLeft() + (fromNow ? DateTime.UtcNow.Day : 0);
            month = ParseToTheLeft() + (fromNow ? DateTime.UtcNow.Month : 0);
            year = ParseToTheLeft() + (fromNow ? DateTime.UtcNow.Year : 0);

            if (day <= 0 || month <= 0 || year <= 0)
                throw new ArgumentException($"Parsed timestamp have wrong values! Day, Month or Year must be higher than 0!\n[More info]\nsecond: {second}, minute: {minute}, hour: {hour}, day: {day}, month: {month}, year: {year}.\n[end]", dateText);

            DateTimeOffset timespan = new DateTimeOffset();
            timespan = timespan.AddSeconds(second)
                .AddMinutes(minute)
                .AddHours(hour)
                .AddDays(day - 1)
                .AddMonths(month - 1)
                .AddYears(year - 1);
            /*
            Console.WriteLine($"second: {timespan.Second}, minute: {timespan.Minute}, hour: {timespan.Hour}, day: {timespan.Day}, month: {timespan.Month}, year: {timespan.Year}");
            */
            return timespan;

            int ParseToTheLeft()
            {
                if (index > 0)
                    index--;
                else
                    return 0;

                return int.Parse(results[index]);
            }
        }

        static Discord.Color ToColor(this string hex)
        {
            bool startWith = hex[0] == '#';

            if (startWith)
                hex = hex.Substring(1);

            if (hex.Length == 6)
            {
                int r, g, b;
                r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                return (Discord.Color) Tools.CreateColor(r, g, b);
            }
            else
                throw new XmlStructureException("Bad Hex color format! use: 'RRGGBB', or '#RRGGBB'.", filename);
        }

        static ButtonStyle ToButtonStyle(this string str)
        {
            str = str.ToLower();
            return str switch
            {
                "primary" => ButtonStyle.Primary,
                "secondary" => ButtonStyle.Secondary,
                "success" => ButtonStyle.Success,
                "danger" => ButtonStyle.Danger,
                "link" => ButtonStyle.Link,
                _ => ButtonStyle.Primary
            };
        }

        static TextInputStyle ToTextStyle(this string str)
        {
            str = str.ToLower();
            return str switch
            {
                "short" => TextInputStyle.Short,
                "paragraph" => TextInputStyle.Paragraph,
                _ => TextInputStyle.Short
            };
        }

        static bool ToBool(this string str)
        {
            str = str.ToLower();
            if (str == "true") return true;
            if (str == "false") return false;
            throw new XmlStructureException($"{nameof(str)} is not valid boolean! Value: {str}", filename);
        }
    }
}       