using System.Threading.Tasks;

namespace NuffBot
{
  public class DatabaseObject<T>
    where T : class, IDatabaseObject
  {
    public T Entity { get; }
    
    public DatabaseObject(T entity)
    {
      Entity = entity;
    }

    public async Task<bool> UpdateInDatabase(IDatabaseContext database)
    {
      return await database.UpdateAsync(Entity);
    }
    
    public async Task<bool> DeleteFromDatabase(IDatabaseContext database)
    {
      return await database.DeleteEntityAsync(Entity);
    }
  }
}