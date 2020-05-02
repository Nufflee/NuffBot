using System.Threading.Tasks;

namespace NuffBot
{
  public abstract class DatabaseModelBase<T>
    where T : DatabaseModelBase<T>
  {
    public virtual void OnUpdateInDatabase()
    {
    }

    public virtual void OnSaveToDatabase()
    {
    }

    public Task<bool> SaveToDatabase(IDatabaseContext database)
    {
      OnSaveToDatabase();

      return database.WriteAsync((T) this);
    }
  }
}