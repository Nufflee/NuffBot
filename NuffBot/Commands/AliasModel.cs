using ServiceStack.DataAnnotations;

namespace NuffBot.Commands
{
  public class AliasModel : DatabaseModel<AliasModel>
  {
    [ForeignKey(typeof(CommandModel), OnDelete = "CASCADE")]
    public ulong CommandId { get; set; }

    public string Alias { get; set; }

    public AliasModel(ulong commandId, string alias)
    {
      CommandId = commandId;
      Alias = alias;
    }
  }
}