using System;
using ServiceStack.DataAnnotations;

namespace NuffBot.Commands
{
  public class CommandModel : DatabaseModelBase<CommandModel>
  {
    [PrimaryKey, AutoIncrement] public ulong Id { get; set; }
    [Unique, Required] public string Name { get; set; }
    [Required] public string Response { get; set; }
    public DateTime CreationTimestamp { get; set; }
    public DateTime LastUpdateTimestamp { get; set; }

    public CommandModel(string name, string response)
    {
      Name = name;
      Response = response;
    }

    public override void OnSaveToDatabase()
    {
      CreationTimestamp = LastUpdateTimestamp = DateTime.UtcNow;
    }

    public override void OnUpdateInDatabase()
    {
      LastUpdateTimestamp = DateTime.UtcNow;
    }
  }
}