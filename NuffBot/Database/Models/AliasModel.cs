using System;
using ServiceStack.DataAnnotations;

namespace NuffBot.Commands
{
  public class AliasModel : DatabaseModelBase<AliasModel>
  {
    [PrimaryKey] public string Alias { get; set; }

    [Required, ForeignKey(typeof(CommandModel), OnDelete = "CASCADE")]
    public ulong CommandId { get; set; }

    public DateTime CreationTimestamp { get; set; }

    public AliasModel(CommandModel command, string alias)
    {
      CommandId = command.Id;
      Alias = alias;
    }

    public override void OnSaveToDatabase()
    {
      CreationTimestamp = DateTime.UtcNow;
    }
  }
}