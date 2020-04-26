using System;
using System.Threading.Tasks;
using NuffBot.Core.Twitch;
using TwitchLib.Api.Helix.Models.Users;

namespace NuffBot.Commands
{
  public class FollowageCommand : Command
  {
    public override string Name => "followage";
    public override UserLevel UserLevel => UserLevel.Viewer;

    private const string Usage = "Usage: !followage [username] - Checks followage of a user.";

    protected override async Task ExecuteTwitch(TwitchChatMessage message, CommandContext context, TwitchBot bot)
    {
      CommandParser parser = CommandParser.TryCreate(message.Content);

      string userName;

      try
      {
        userName = parser?.ParseOptionalWord();
      }
      catch (CommandParseError error)
      {
        bot.SendMessage(error.Message, context);
        bot.SendMessage(Usage, context);

        return;
      }

      string fromId;

      if (userName == null)
      {
        fromId = message.Sender.Id.ToString();
      }
      else
      {
        fromId = TwitchUser.GetByName(userName).Id.ToString();
      }

      GetUsersFollowsResponse follow = await TwitchBot.TwitchApi.Helix.Users.GetUsersFollowsAsync(fromId: fromId, toId: TwitchBot.CurrentChannel.Id);

      if (follow.Follows.Length == 0)
      {
        if (userName == null)
        {
          bot.SendMessage($"@{message.Sender.UserName} You are not following the channel.", context);
        }
        else
        {
          bot.SendMessage($"@{message.Sender.UserName} {userName} is not following the channel.", context);
        }
      }
      else
      {
        TimeSpan followage = DateTime.UtcNow - follow.Follows[0].FollowedAt;

        if (userName == null)
        {
          bot.SendMessage($"@{message.Sender.UserName} You have been following the channel for {Helpers.FormatTimeSpan(followage)}.", context);
        }
        else
        {
          bot.SendMessage($"@{message.Sender.UserName} {userName} has been following the channel for {Helpers.FormatTimeSpan(followage)}.", context);
        }
      }
    }
  }
}