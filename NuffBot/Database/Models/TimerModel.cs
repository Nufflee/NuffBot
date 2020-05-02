using System;
using NuffBot.Commands;
using ServiceStack.DataAnnotations;

namespace NuffBot.Core
{
  public class TimerModel : DatabaseModelBase<TimerModel>
  {
    [PrimaryKey, AutoIncrement] public ulong Id { get; set; }

    [Required, ForeignKey(typeof(CommandModel), OnDelete = "CASCADE")]
    public ulong CommandId { get; set; }

    public int TimeTrigger { get; set; }
    public int MessageTrigger { get; set; }
    public DateTime CreationTimestamp { get; set; }
    public DateTime LastUpdateTimestamp { get; set; }

    public TimerModel(CommandModel command, int timeTrigger, int messageTrigger)
    {
      CommandId = command.Id;
      TimeTrigger = timeTrigger;
      MessageTrigger = messageTrigger;
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