using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace NuffBot.Commands
{
  public class DatabaseCommand : IDatabaseObject
  {
    [AutoIncrement] public ulong Id { get; set; }
    [Unique] public string Name { get; set; }
    public List<string> Aliases { get; set; }
    public string Response { get; set; }

    public DatabaseCommand(string name, List<string> aliases, string response)
    {
      Name = name;
      Aliases = aliases;
      Response = response;
    }

    public Task<bool> SaveToDatabase(IDatabaseContext database)
    {
      return database.WriteAsync(this);
    }
  }
}