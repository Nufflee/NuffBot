using NuffBot.Core;

namespace NuffBot
{
  public abstract class Bot
  {
    public abstract ulong Id { get; protected set; }

    public abstract void SendMessage(string message, CommandContext context);
    public abstract void SendPrivateMessage(User receiver, string message, CommandContext context);
  }
}