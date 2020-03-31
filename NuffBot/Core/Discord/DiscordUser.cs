using System.Linq;
using Discord.WebSocket;
using NuffBot.Core;

namespace NuffBot.Discord
{
  public class DiscordUser : User
  {
    // public TwitchUser TwitchUser { get; }

    public DiscordUser(SocketGuildUser user)
      : this(GetUserLevel(user), user.Username, user.Id)
    {
    }

    private DiscordUser(UserLevel userLevel, string userName, ulong id)
      : base(userLevel, userName, id)
    {
    }

    private static UserLevel GetUserLevel(SocketGuildUser user)
    {
      SocketRole highestRole = user.Roles.OrderBy(role => role.Position).Last();

      switch (highestRole.Name)
      {
        case "Owner":
          return UserLevel.God;
        case "Moderator":
          return UserLevel.Moderator;
        case "VIP":
          return UserLevel.VIP;
        case "Twitch Subscriber":
        case "Nitro Booster":
          return UserLevel.Subscriber;
        default:
          return UserLevel.Viewer;
      }
    }
  }
}