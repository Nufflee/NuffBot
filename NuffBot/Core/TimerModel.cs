using NuffBot.Commands;
using ServiceStack.DataAnnotations;

namespace NuffBot.Core
{
  public class TimerModel : DatabaseModel<TimerModel>
  {
    [ForeignKey(typeof(CommandModel), OnDelete = "CASCADE")]
    public ulong CommandId { get; set; }

    public int TimeTrigger { get; set; }
    public int MessageTrigger { get; set; }

    public TimerModel(ulong commandId, int timeTrigger, int messageTrigger)
    {
      CommandId = commandId;
      TimeTrigger = timeTrigger;
      MessageTrigger = messageTrigger;
    }
  }
}