using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Api.V5.Models.Users;

namespace NuffBot.Core.Twitch
{
  public class TwitchUser : User
  {
    private TwitchUser(UserLevel userLevel, string userName, ulong id)
      : base(userLevel, userName, id)
    {
    }

    public static TwitchUser GetByName(string name, UserLevel userLevel = UserLevel.Unknown)
    {
      Users users = TwitchBot.TwitchApi.V5.Users.GetUserByNameAsync(name).Result;

      if (users.Matches.Length == 0)
      {
        return null;
      }

      if (userLevel == UserLevel.Unknown)
      {
        ChatterFormatted chatter = TwitchBot.TwitchApi.Undocumented.GetChattersAsync(Configuration.TwitchChannelName).Result.Find(c => c.Username == name);

        if (chatter != null)
        {
          userLevel = chatter.UserType.ToUserLevel();
        }
      }

      TwitchLib.Api.V5.Models.Users.User user = users.Matches[0];
      
      return new TwitchUser(userLevel, user.DisplayName, ulong.Parse(user.Id));
    }

    public static TwitchUser GetById(ulong id, UserLevel userLevel = UserLevel.Unknown)
    {
      TwitchLib.Api.V5.Models.Users.User user = TwitchBot.TwitchApi.V5.Users.GetUserByIDAsync(id.ToString()).Result;

      if (user == null)
      {
        return null;
      }

      if (userLevel == UserLevel.Unknown)
      {
        ChatterFormatted chatter = TwitchBot.TwitchApi.Undocumented.GetChattersAsync(Configuration.TwitchChannelName).Result.Find(c => c.Username == user.Name);
        
        if (chatter != null)
        {
          userLevel = chatter.UserType.ToUserLevel();
        }
      }

      return new TwitchUser(userLevel, user.DisplayName, ulong.Parse(user.Id));
    }
  }
}