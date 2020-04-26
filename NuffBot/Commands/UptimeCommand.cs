using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Streams;

namespace NuffBot.Commands
{
  public class UptimeCommand : Command
  {
    public override string Name => "uptime";
    public override UserLevel UserLevel => UserLevel.Viewer;

    private const string Usage = "Usage: !uptime - Shows the uptime of the stream.";

    protected override async Task Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      GetStreamsResponse response = await TwitchBot.TwitchApi.Helix.Streams.GetStreamsAsync(userIds: new List<string> {TwitchBot.CurrentChannel.Id});

      if (response.Streams.Length > 0)
      {
        TimeSpan uptime = DateTime.UtcNow - response.Streams[0].StartedAt;

        bot.SendMessage($"@{message.Sender.UserName} Stream has been live for {Helpers.FormatTimeSpan(uptime)}.", context);
      }
      else
      {
        bot.SendMessage($"@{message.Sender.UserName} Stream is offline.", context);
      }
    }
  }
}