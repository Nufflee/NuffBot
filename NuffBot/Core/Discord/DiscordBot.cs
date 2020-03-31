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

      client.LoginAsync(TokenType.Bot, Configuration.DiscordToken).Wait();
      client.StartAsync().Wait();

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

    public override void SendMessage(string message, CommandContext context)
    {
      ((DiscordCommandContext) context).DiscordChannel.SendMessageAsync(message).Wait();
    }

    public override void SendPrivateMessage(User receiver, string message, CommandContext context)
    {
      ((DiscordCommandContext) context).DiscordChannel.GetUserAsync(receiver.Id).Result.SendMessageAsync(message);
    }
  }
}