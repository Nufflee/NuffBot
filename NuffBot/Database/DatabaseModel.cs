using System;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace NuffBot
{
  public abstract class DatabaseModel<T>
    where T : DatabaseModel<T>
  {
    [AutoIncrement] public virtual ulong Id { get; set; }
    public virtual DateTime? CreationTimestamp { get; set; }
    public virtual DateTime? LastUpdateTimestamp { get; set; }

    public virtual Task<bool> SaveToDatabase(IDatabaseContext database)
    {
      CreationTimestamp = LastUpdateTimestamp = DateTime.UtcNow;
      
      return database.WriteAsync((T)this);
    }
  }
}