using Discord;
using Discord.Commands;
using Discord.Extended;
using System;
using System.Threading.Tasks;

public class Commands : Extended<SocketCommandContext>
{
    [Command("test")]
    public async Task Test()
    {
        var x = new PagedMessageCreator()
           .AddPage("1/3", "Od rozmowy między Tymkiem i Neraldem, a Fate'm minęło już sześć godzin, które zostały poświęcone nowo odkrytej wyrwie - Znajdowała się w samym środku stołówki statku badawczego, co umożliwiło na łatwe rozstawienie specjalnych przyrządów analizujących, przystosowanych do badania wyrw.")
           .AddPage("2/3", " Było to możliwe tylko dlatego, że parę miesięcy wstecz doszło do całkiem podobnego zjawiska, lecz tamtejsza wyrwa była strasznie niestabilna, dlatego powstały przyrządy umożliwiające ustabilizowanie jej kosztem zmniejszenia do niewyobrażalnie małych rozmiarów - Ten przypadek jest inny.")
           .AddPage("3/3", "Fate, nieustannie zdumiony kompletnie losowym powstaniem tak specyficznej wyrwy, siedział przy jednej ze stołówkowych ławek, trzymając długopis i kartkę papieru.")
           .AddInteractiveIEmote(new Emoji("◀️"), PagedEvents.PreviousPage)
           .AddInteractiveIEmote(new Emoji("▶️"), PagedEvents.NextPage);

        x.OnMessageOutdated = MessageOutdated;

        //x.AddInteractiveIEmote(new Emoji(""), PagedEvents.SetPage(this,1)).Build();

        var pagedMessage = x.Build();

        await SendPagedMessage(pagedMessage);
    }

    void MessageOutdated()
    {
        Console.WriteLine("tak było");
    }
}