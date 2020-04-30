using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace NuffBot.Commands
{
  public class CommandModel : DatabaseModel
  {
    [Unique] public string Name { get; set; }
    public List<string> Aliases { get; set; }
    public string Response { get; set; }

    public CommandModel(string name, List<string> aliases, string response)
    {
      Name = name;
      Aliases = aliases;
      Response = response;
    }
  }
}