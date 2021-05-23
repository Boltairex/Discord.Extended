# Discord.Extended
[![NuGet](https://img.shields.io/nuget/vpre/Discord-Extended.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord-Extended)
[![Discord](https://discord.com/api/guilds/846043238649298944/widget.png)](https://discord.gg/jkrBmQR)

Library, that's extends Discord.net via adding features like paged messages.

# Showcase

![Sample of our library!](https://cdn.discordapp.com/attachments/817822681050120256/846039535103901726/discord.extended.gif)

And here is code behind that gif:

```cs
[Command("test")]
public async Task Test()
{
    var x = new PagedMessageCreator(10, Context.User)
       .AddPage("1/3", "Page one")
       .AddPage("2/3", "Page two")
       .AddPage("3/3", "Page Three")
       .AddInteractiveIEmote(new Emoji("◀️"), PagedEvents.PreviousPage)
       .AddInteractiveIEmote(new Emoji("▶️"), PagedEvents.NextPage)
       .SetLifetime(30);

    var pagedMessage = x.Build();

    await SendPagedMessage(pagedMessage);
}
```

# Usage

Read wiki for explanation of this library!
