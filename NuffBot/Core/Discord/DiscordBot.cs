using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using NuffBot.Commands;
using NuffBot.Core;
using ServiceStack;

namespace NuffBot.Discord
{
  public class DiscordBot : Bot
  {
    public override ulong Id { get; protected set; }
    public static IGuild CurrentGuild { get; private set; }

    private readonly DiscordSocketClient client;

    public DiscordBot()
    {
      client = new DiscordSocketClient();

      Connect();
    }

    private async void Connect()
    {
      client.LoginAsync(TokenType.Bot, Configuration.DiscordToken).Wait();
      await client.StartAsync();

      client.Ready += WhenReady;
      client.MessageReceived += OnMessageReceived;
    }
    
    private Task WhenReady()
    {
      Console.WriteLine($"Discord bot connected!");

      CurrentGuild = client.Guilds.FirstNonDefault();
      Id = client.CurrentUser.Id;

      return Task.CompletedTask;
    }

    private Task OnMessageReceived(SocketMessage message)
    {
      Stopwatch sw = Stopwatch.StartNew();

      if (message.Content.StartsWith("!"))
      {
        CommandProcessor.ProcessCommand(new DiscordChatMessage(message), this);
      }

      Console.WriteLine($"It took {sw.ElapsedMilliseconds} ms to process Discord message.");

      return Task.CompletedTask;
    }

    public override async void SendMessage(string message, CommandContext context)
    {
      await ((DiscordCommandContext) context).DiscordChannel.SendMessageAsync(message);
    }

    public override async void SendPrivateMessage(User receiver, string message, CommandContext context)
    {
      await (await ((DiscordCommandContext) context).DiscordChannel.GetUserAsync(receiver.Id)).SendMessageAsync(message);
    }
  }
}