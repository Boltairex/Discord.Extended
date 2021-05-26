# Discord.Extended

[![NuGet](https://img.shields.io/nuget/vpre/Discord-Extended.svg?maxAge=2592000?style=plastic)](https://www.nuget.org/packages/Discord-Extended)
[![Discord](https://discord.com/api/guilds/846043238649298944/widget.png)](https://discord.gg/jkrBmQR)

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/built-with-love.svg)](https://forthebadge.com)

Library that extends [Discord.net](https://github.com/discord-net/Discord.Net) via adding features like paged messages with minor optimization of changing messages content (because of ratelimit), and more features that can extend your bot's functionality.

# Min. Requirements
* Version of 2.3.0 (previous versions untested!) for Discord.Net (NuGet gets it automatically)
* Visual Studio Community 2017 or 2019

# Installation
You have a few options to install the library.
* Check [NuGet website](https://www.nuget.org/packages/Discord-Extended) or NuGet Package Manager in Visual Studio by phrase 'Discord-Extended'.
* Install from [releases](https://github.com/Boltairex/Discord.Extended/releases) and manually add a DLL file to your Visual Studio Project.
* Get Source code and compile/modify it on your own.

# Why should you use Discord.Extended?

When you:
* Like interactivity,
* Enjoy pretty looking messages (and embeds!),
* Demand an easy to use library, 
* Are tired of Discord's ratelimit while sending messages too often (it still may happen, though less),

Well, then you should try! There's even a documentation located in the [Wiki](https://github.com/Boltairex/Discord.Extended/wiki) category for you to use.
Our message building is similar to Discord.Net's `EmbedBuilder`, in case if any *experienced person* is wondering.

If you are in need of help, don't hestitate to join our [Discord](https://discord.gg/xgbEffMnVw) server!

# Showcase

![Sample of our library!](https://cdn.discordapp.com/attachments/817822681050120256/846039535103901726/discord.extended.gif)

And the code behind it:

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

This is the simplest way of implementing a `PagedMessage`. We also prepared some simple prefabs for message controlling.

# Usage

Read the Wiki tab for explanation!

# About Creators (Boltairex & Qzername)

We are C# programmers since 2018/2017, and we worked with various things, like Rest API for websites, games on Unity, and smaller programs/tools. We also have some experience with making bots (since 2019-2020), but it's our first published library. We would appreciate feedback and ideas for further library development.
