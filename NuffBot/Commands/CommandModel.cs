using ServiceStack.DataAnnotations;

namespace NuffBot.Commands
{
  public class CommandModel : DatabaseModel<CommandModel>
  {
    [Unique] public string Name { get; set; }
    public string Response { get; set; }

    public CommandModel(string name, string response)
    {
      Name = name;
      Response = response;
    }
  }
}