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
            while (en.MoveNext())
            {
                switch (en.Current.Name)
                {
                    case "option1":
                        if (en.Current.Value is string text)
                            await interaction.RespondAsync(text: "You said: " + text);
                        break;
                }
            }
        }

        protected override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("Testing something")
                .AddOption("option1", ApplicationCommandOptionType.String, "Accept string", false); ;
        }
    }
}
