using Discord;
using TwitchLib.Api.V5.Models.Channels;

namespace NuffBot
{
  public class CommandContext
  {
    public IGuild DiscordGuild { get; }
    public Channel TwitchChannel { get; }

    public CommandContext(IGuild discordGuild, Channel twitchChannel)
    {
      DiscordGuild = discordGuild;
      TwitchChannel = twitchChannel;
    }
  }
}