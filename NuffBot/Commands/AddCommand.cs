namespace NuffBot.Commands
{
  public class AddCommand : Command
  {
    public override string Name => "addcmd";
    public override UserLevel UserLevel => UserLevel.God;

    private const string usage = "Usage: !addcmd <name> [<aliases>] <response> - Adds a command to the database.";

    protected override void Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      CommandParser parser = CommandParser.TryCreate(message.Content);

      if (parser == null)
      {
        bot.SendMessage(usage, context);

        return;
      }

      string name;
      string[] aliases;
      string response;

      try
      {
        name = parser.ParseWord();
        aliases = parser.ParseOptionalArray();
        response = parser.ParseRest();
      }
      catch (CommandParseError error)
      {
        bot.SendMessage(error.Message, context);
        bot.SendMessage(usage, context);

        return;
      }

      // do db stuff

      bot.SendMessage($"Command '{name}' successfully added!", context);
    }
  }
}