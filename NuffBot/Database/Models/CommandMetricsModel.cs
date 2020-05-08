using System;
using NuffBot.Commands;
using ServiceStack.DataAnnotations;

namespace NuffBot.Database.Models
{
  public class CommandMetricsModel : DatabaseModelBase<CommandMetricsModel>
  {
    [PrimaryKey, AutoIncrement] public ulong Id { get; set; }
    public DateTime Date { get; set; }

    [Required, ForeignKey(typeof(CommandModel))]
    public ulong CommandId { get; set; }

    public string CommandName { get; set; }
    [Required] public uint ExecutionCount { get; set; }

    public CommandMetricsModel(CommandModel command)
    {
      CommandId = command.Id;
      CommandName = command.Name;
      ExecutionCount = 1;
    }

    public override void OnSaveToDatabase()
    {
      Date = DateTime.UtcNow;
    }
  }
}