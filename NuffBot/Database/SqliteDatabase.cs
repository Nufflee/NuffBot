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

    public async Task<T> ReadAsync<T>(uint id)
      where T : class, IDatabaseObject
    {
      return await Connection.LoadSingleByIdAsync<T>(id);
    }

    public async Task<List<T>> ReadAllAsync<T>()
      where T : class, IDatabaseObject
    {
      return await Connection.LoadSelectAsync(Connection.From<T>());
    }

    public async Task<List<T>> ReadAllAsync<T>(uint id)
      where T : class, IDatabaseObject
    {
      return await Connection.LoadSelectAsync<T>(x => x.Id == id);
    }

    public List<T> Select<T>(Expression<Func<T, bool>> predicate)
    {
      return Connection.Select(predicate);
    }
    
    public void Dispose()
    {
      Connection?.Dispose();
    }
  }
}