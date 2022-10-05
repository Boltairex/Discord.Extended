using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Extended.Applications;
using Discord.WebSocket;
using Discord.Extended;
using Discord.Extended.Models;

namespace BotForTest.SlashCommands.Test
{
    public class TestXmlCommands : SlashCommandBase
    {
        //Store your models
        Modal modal;
        MessageComponent component;
        EmbedBuilder embed;

        public override async Task ExecuteAsync(SocketSlashCommand interaction, SocketUser author)
        {
            try
            {
                var en = interaction.Data.Options.GetEnumerator();
                while (en.MoveNext())
                {
                    // By Name
                    if (en.Current.Name == "choices")
                    {
                        Int64 val = (Int64)en.Current.Value;
                        Console.WriteLine(val);
                        if (val == 0)
                            await interaction.RespondWithModalAsync(modal);
                        else if (val == 1)
                            await interaction.RespondAsync(components: component);
                        else if (val == 2)
                            await interaction.RespondAsync(embed: embed.WithAuthor(author).Build());
                    }
                }

                if (!interaction.HasResponded)
                    await interaction.RespondAsync("Select type.");
            }catch(Exception e) { Console.WriteLine(e); }
            }

        protected override SlashCommandBuilder GetCommand()
        {
            var en = XmlToComponents.CreateComponentsFromXMLFile("C:/Users/USERNAME/Desktop/test.xml");
            while (en.MoveNext())
            {
                if (en.IsModal)
                    modal = en.CurrentAsModal;
                else if (en.IsMessageComponent)
                    component = en.CurrentAsMessageComponent;
                else if (en.IsEmbedBuilder)
                    embed = en.CurrentAsEmbedBuilder;
            }

            return new SlashCommandBuilder()
                .WithName("xml")
                .WithDescription("Show XML type")
                .AddOption(Tools.FormChoices("choices", "desc", true, "modal", "component", "embed"));
        }
    }
}
