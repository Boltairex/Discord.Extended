# Discord.Extended [Unsupported]
[![Nuget](https://img.shields.io/nuget/v/Discord.Extended.Core)](https://www.nuget.org/packages/Discord.Extended.Core)
[![Nuget](https://img.shields.io/nuget/dt/Discord.Extended.Core)](https://www.nuget.org/packages/Discord.Extended.Core)

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com) [![forthebadge](https://forthebadge.com/images/badges/built-with-love.svg)](https://forthebadge.com)

Library that extends [Discord.net](https://github.com/discord-net/Discord.Net) via adding features like ApplicationCommands handler, paged messages with minor optimization of changing messages content (because of ratelimit), and more features that can extend your bot's functionality.

Due to NuGet push errors, we created new package (older is named Discord.Extended, new Discord.Extended.Core).

# Currently working on (version 2.2.0)
* Extending ApplicationCommands system (like better subcommands creation, not direct commands). Includes receiving events by specified customId.
* Better SubCommands in SlashCommands, smart auto filling methods in components.

# Version 2.1.0
* Xml structure parsing to: [Modal](https://discordnet.dev/guides/int_basics/modals/intro.html) or [MessageComponent](https://discordnet.dev/guides/int_basics/message-components/intro.html).
* Tools improvement

**Previews:**
* [XML structure](https://media.discordapp.net/attachments/1018240922896576612/1019673167301709824/unknown.png)
* [Modal result](https://cdn.discordapp.com/attachments/1018240922896576612/1019690024444370996/unknown.png)

# Discord.Net.Core (>= 2.0.0) min. Requirements
* C#, netstandard 2.1
* Discord.Net >= 3.7.0
* Microsoft.Dependency.Injection >= 5.0.2

# Discord.Net (1.0.0) min. Requirements
* C#, netstandard 2.0
* Discord.Net = 2.\*.\* (tested on 2.3.0)
* Visual Studio Community 2017 or 2019

# Installation
You have a few options to install the library.
* Check [NuGet website](https://www.nuget.org/packages/Discord.Extended.Core) or NuGet Package Manager in Visual Studio by phrase 'Discord-Extended'.
* Install from [releases](https://www.nuget.org/packages/Discord.Extended.Core/releases) and manually add a DLL file to your Visual Studio Project.
* Get Source code and compile/modify it on your own.

# Why should you use Discord.Extended?

When you:
* Are tired of massive boilerplate due to strange ApplicationCommands design,
* Like simple and useful tools,
* Like interactivity,
* Enjoy pretty looking messages, embeds and UI's,
* Like easy library extension, 
* Demand an easy to use library, 
* Are tired of Discord's ratelimit while sending messages too often (it still may happen, though less),

Well, then you should try! There's even a documentation located in the [Wiki](https://github.com/Boltairex/Discord.Extended/wiki) category for you to use.
Our message building is similar to Discord.Net's `EmbedBuilder`, in case if any *experienced person* is wondering.

If you are in need of help, don't hestitate to create an [issue](https://github.com/Boltairex/Discord.Extended/issues)!

# Setting up ApplicationCommandsService 
First, preparing our ApplicationCommandService (can be initialized via ServiceCollection). [Code from](https://github.com/Boltairex/Discord.Extended/blob/main/BotForTest/Program.cs) our test bot.

```cs
... Bot login scope
// Use this code before using any command.

    Console.WriteLine("Update application commands? T/*");
    var key = Console.ReadKey();
    bool update = key.Key == ConsoleKey.T;
    
    ApplicationCommandService application = new ApplicationCommandService(client);
    application.CollectSlashCommands();
    application.CollectUserCommands();
    application.CollectMessageCommands();
        
    client.Ready += () => {
        application.RegisterCommands(update, true);
        return Task.CompletedTask;
    };

...
```

Creating ApplicationCommand (in this case, SlashCommand, [code from](https://github.com/Boltairex/Discord.Extended/blob/main/BotForTest/SlashCommands/TestCommand.cs))
```cs
    public class TestCommand : SlashCommandBase
    {
        // If not overriden, it will be created as global command
        public override ulong[] GuildsID => new ulong[] { 913898440025579541 };

        // After calling your command
        public override async Task ExecuteAsync(SocketSlashCommand interaction, SocketUser author)
        {
            var en = interaction.Data.Options.GetEnumerator();
            while(en.MoveNext())
            {
                switch(en.Current.Name)
                {
                    case "option1":
                        if (en.Current.Value is string text)
                            await interaction.RespondAsync(text: "you wrote " + text);
                        break;
                }
            }
        }

        // Slash Command structure builder
        protected override SlashCommandBuilder GetCommand()
        {
            SlashCommandBuilder builder = new SlashCommandBuilder()
                .WithName("test")
                .WithDescription("testing something")
                .AddOption("option1", ApplicationCommandOptionType.String, "get text", false);

            return builder;
        }
    }
```
One issue. Global commands sometimes takes long to register, in testing purpose set your GuildsID to update immediately. Also, ApplicationCommandService is prepared for easy extending, and you can just override it's methods by yourself.

# Showcase of PagedMessage

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
