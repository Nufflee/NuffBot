using System.Threading.Tasks;

namespace NuffBot
{
  public class DatabaseObject<T>
    where T : DatabaseModelBase<T>
  {
    public static DatabaseObject<T> Empty => new DatabaseObject<T>(null);
    
    public T Entity { get; }
    
    public DatabaseObject(T entity)
    {
      Entity = entity;
    }

    public async Task<bool> UpdateInDatabase(IDatabaseContext database)
    {
      Entity.OnUpdateInDatabase();

      return await database.UpdateAsync(Entity);
    }
    
    public async Task<bool> DeleteFromDatabase(IDatabaseContext database)
    {
      return await database.DeleteEntityAsync(Entity);
    }

    public bool Exists()
    {
      return Entity != null;
    }
  }
}