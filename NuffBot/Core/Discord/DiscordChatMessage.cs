using Discord.WebSocket;

namespace NuffBot.Discord
{
  public class DiscordChatMessage : ChatMessage<DiscordUser>
  {
    public ISocketMessageChannel Channel { get; }

    public DiscordChatMessage(SocketMessage message)
      : base(new DiscordUser((SocketGuildUser)message.Author), message.Id.ToString(), message.Content, message.Timestamp.DateTime)
    {
      Channel = message.Channel;
    }
  }
}