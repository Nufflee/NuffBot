using System.Threading.Tasks;
using NuffBot.Commands;
using ServiceStack.DataAnnotations;

namespace NuffBot.Core
{
  public class Timer : IDatabaseObject
  {
    [AutoIncrement] public ulong Id { get; set; }

    [ForeignKey(typeof(DatabaseCommand), OnDelete = "CASCADE")]
    public ulong CommandId { get; set; }
    public int TimeTrigger { get; set; }
    public int MessageTrigger { get; set; }

    public Timer(ulong commandId, int timeTrigger, int messageTrigger)
    {
      CommandId = commandId;
      TimeTrigger = timeTrigger;
      MessageTrigger = messageTrigger;
    }

    public Task<bool> SaveToDatabase(IDatabaseContext database)
    {
      return database.WriteAsync(this);
    }
  }
}