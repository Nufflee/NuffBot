using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;

namespace NuffBot
{
  public class SqliteDatabase : IDatabaseContext
  {
    public static SqliteDatabase Instance { get; private set; }

    public IDbConnection Connection { get; }

    private SqliteDatabase(string path)
    {
      OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(path, SqliteOrmLiteDialectProvider.Instance);
      Connection = factory.OpenDbConnection();

      if (Connection.State != ConnectionState.Open)
      {
        Console.WriteLine("Failed to connect to the database!");
      }

      foreach (Type tableType in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => typeof(IDatabaseObject).IsAssignableFrom(t) && !t.IsInterface).ToArray())
      {
        Connection.CreateTableIfNotExists(tableType);
      }
    }

    public static void Connect(string path)
    {
      Instance = new SqliteDatabase(path);
    }

    public async Task<bool> WriteAsync<T>(T entity)
      where T : class, IDatabaseObject
    {
      return await Connection.SaveAsync(entity, true);
    }

    public async Task<bool> DeleteAsync<T>(T entity)
      where T : class, IDatabaseObject
    {
      // This may need to be != 0.
      return await Connection.DeleteAsync(entity) > 0;
    }

    public async Task<DatabaseObject<T>> ReadSingleAsync<T>(Expression<Func<T, bool>> predicate)
      where T : class, IDatabaseObject
    {
      T item = await Connection.SingleAsync(predicate);
      
      return new DatabaseObject<T>(item);
    }

    public async Task<List<DatabaseObject<T>>> ReadAllAsync<T>()
      where T : class, IDatabaseObject
    {
      List<T> items = await Connection.LoadSelectAsync(Connection.From<T>());
      
      return items.Select((x) => new DatabaseObject<T>(x)).ToList();
    }

    public async Task<List<DatabaseObject<T>>> ReadAllAsync<T>(Expression<Func<T, bool>> predicate)
      where T : class, IDatabaseObject
    {
      List<T> items = await Connection.LoadSelectAsync<T>(predicate);
      
      return items.Select((x) => new DatabaseObject<T>(x)).ToList();
    }

    public async Task<bool> UpdateAsync<T>(T entity)
      where T : class, IDatabaseObject
    {
      int result = await Connection.UpdateAsync(entity);
      
      return result == 1;
    }

    public void Dispose()
    {
      Connection?.Dispose();
    }
  }
}