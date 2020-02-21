using System;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace NuffBot.Core.Twitch
{
  public class TwitchChatMessage : ChatMessage<TwitchUser>
  {
    public TwitchChatMessage(ChatMessage chatMessage)
      : base(TwitchUser.GetByName(chatMessage.DisplayName, chatMessage.GetUserLevel()), chatMessage.Id, chatMessage.Message,
        DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(Regex.Match(chatMessage.RawIrcMessage, "tmi-sent-ts=(.*?);").Groups[1].Value)).ToUniversalTime().DateTime)
    {
    }
  }
}