using TwitchLib.Api.Core.Enums;
using TwitchLib.Client.Models;

namespace NuffBot
{
  public static class TwitchLibExtensions
  {
    public static UserLevel GetUserLevel(this ChatMessage message)
    {
      UserLevel userLevel = UserLevel.Unknown;

      if (message.UserType == TwitchLib.Client.Enums.UserType.Viewer)
      {
        userLevel = UserLevel.Viewer;
      }

      if (message.IsSubscriber)
      {
        userLevel = UserLevel.Subscriber;
      }

      if (message.IsModerator)
      {
        userLevel = UserLevel.Moderator;
      }

      if (message.IsBroadcaster)
      {
        userLevel = UserLevel.God;
      }

      return userLevel;
    }

    public static UserLevel ToUserLevel(this UserType userType)
    {
      switch (userType)
      {
        case UserType.Viewer:
          return UserLevel.Viewer;
        case UserType.VIP:
          return UserLevel.VIP;
        case UserType.Moderator:
          return UserLevel.Moderator;
        case UserType.Broadcaster:
          return UserLevel.God;
        default:
          return UserLevel.Unknown;
      }
    }
  }
}