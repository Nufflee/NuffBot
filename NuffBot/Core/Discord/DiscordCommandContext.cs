using Discord;
using TwitchLib.Api.V5.Models.Channels;

namespace NuffBot.Discord
{
  public class DiscordCommandContext : CommandContext
  {
    public IMessageChannel DiscordChannel { get; }
    
    public DiscordCommandContext(CommandContext context, IMessageChannel discordChannel) 
      : base(context.DiscordGuild, context.TwitchChannel)
    {
      DiscordChannel = discordChannel;
    }
  }
}