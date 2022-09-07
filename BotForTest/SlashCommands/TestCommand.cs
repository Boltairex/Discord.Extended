using Discord;
using Discord.Extended.Models;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BotForTest.SlashCommands
{
    public class TestCommand : SlashCommandBase
    {
        public override ulong[] GuildsID => new ulong[] { 913898440025579541 };

        public override async Task ExecuteAsync(SocketSlashCommand interaction, SocketUser author)
        {
            var en = interaction.Data.Options.GetEnumerator();
            while(en.MoveNext())
            {
                switch(en.Current.Name)
                {
                    case "opcja1":
                        if (en.Current.Value is string text)
                            await interaction.RespondAsync(text: "napisałeś " + text);
                        break;
                }
            }
        }

        protected override SlashCommandBuilder GetCommand()
        {
            SlashCommandBuilder builder = new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("testuje cos")
                .AddOption("opcja1", ApplicationCommandOptionType.String, "przyjmuje tekst", false);

            return builder;
        }
    }
}
