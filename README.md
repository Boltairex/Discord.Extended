# Discord.Extended
[![NuGet](https://img.shields.io/nuget/vpre/Discord-Extended.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord-Extended)
[![Discord](https://discord.com/api/guilds/846043238649298944/widget.png)](https://discord.gg/jkrBmQR)

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/built-with-love.svg)](https://forthebadge.com)

Library that extends Discord.net via adding features like paged messages with minor optimization of changing messages content (because of ratelimit), and more features that can extend your bot's functionality.

# Why you should use our library?

When you:
* Like interactivity,
* Like nice looking messages (embeds),
* Like easy to use library, 
* Are tired of discord ratelimit when you changing messages too often (it still may happen but less),

Then you should try. We provided for you an documentation in 'Wiki' tab with simple examples and solutions.
Our message building is similar to Discord.Net `EmbedBuilder`, so you don't have to learn it from the beggining if you used it earlier.

And of course we can help you, if you have more complex questions. Just join us on Discord. 

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

Showcase gif is the simplest way how you can use it, but really you can build powerful manage tools with it in panel form. We also prepared simple prefabs of message controlling.

# Usage

Read Wiki tab for explanation of this library!

# About Creators

We are C# programmers since 2018/2017, and we works with various things, like Rest API for websites, games on Unity, and smaller programs/tools. Also we have some experience with creating bots (since 2019-2020), but it's our first published library. We would appreciate feedback and ideas for longer library development.
