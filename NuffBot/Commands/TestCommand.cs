namespace NuffBot.Commands
{
  public class TestCommand : Command
  {
    public override string Name => "test";

    protected override void Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      bot.SendMessage("!test", context);
    }
  }
}