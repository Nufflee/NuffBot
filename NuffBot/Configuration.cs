using System;

namespace NuffBot
{
  public static class Configuration
  {
    public static string TwitchChannelName { get; } = GetEnvironmentVariable("TwitchChannelName");
    public static string TwitchBotUsername { get; } = GetEnvironmentVariable("TwitchBotUsername");
    public static string TwitchBotOAuth { get;  } = GetEnvironmentVariable("TwitchBotOAuth");
    public static string TwitchClientId { get; } = GetEnvironmentVariable("TwitchClientId");
    public static string DiscordToken { get; } = GetEnvironmentVariable("DiscordToken");

    private static string GetEnvironmentVariable(string name)
    {
      string value = Environment.GetEnvironmentVariable(name);

      if (value == null)
      {
        throw new Exception($"Couldn't find {name} in .env.");
      }

      return value;
    }
  }
}