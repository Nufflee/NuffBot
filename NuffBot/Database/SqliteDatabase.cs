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

      SqliteOrmLiteDialectProvider.Instance.CreateTableFieldsStrategy = definition =>
      {
        List<FieldDefinition> fieldDefs = new List<FieldDefinition>();

        foreach (FieldDefinition def in definition.FieldDefinitions)
        {
          if (def.Name == "Id")
          {
            fieldDefs.Insert(0, def);
          }
          else
          {
            fieldDefs.Add(def);
          }
        }

        return fieldDefs;
      };

      Connection = factory.OpenDbConnection();

      if (Connection.State != ConnectionState.Open)
      {
        Console.WriteLine("Failed to connect to the database!");
      }

      Connection.ExecuteSql("PRAGMA foreign_keys = ON;");

      foreach (Type tableType in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsAssignableFromGeneric(typeof(DatabaseModel<>)) && t != typeof(DatabaseModel<>)).ToArray())
      {
        Connection.CreateTableIfNotExists(tableType);
      }
    }

    public static SqliteDatabase Connect(string path)
    {
      Instance = new SqliteDatabase(path);

      return Instance;
    }

    public Task<bool> WriteAsync<T>(T entity)
      where T : DatabaseModel<T>
    {
      return Connection.SaveAsync(entity, true);
    }

    public async Task<bool> DeleteEntityAsync<T>(T entity)
      where T : DatabaseModel<T>
    {
      // This may need to be != 0.
      return await Connection.DeleteAsync(entity) > 0;
    }

    public async Task<bool> DeleteAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModel<T>
    {
      // This may need to be != 0.
      return await Connection.DeleteAsync(predicate) > 0;
    }

    public async Task<DatabaseObject<T>> ReadSingleAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModel<T>
    {
      T item = await Connection.SingleAsync(predicate);

      return new DatabaseObject<T>(item);
    }

    public async Task<DatabaseObject<T>> ReadSingleByIdAsync<T>(ulong id)
      where T : DatabaseModel<T>
    {
      T item = await Connection.SingleByIdAsync<T>(id);

      return new DatabaseObject<T>(item);
    }

    public async Task<List<DatabaseObject<T>>> ReadAllAsync<T>()
      where T : DatabaseModel<T>
    {
      List<T> items = await Connection.LoadSelectAsync(Connection.From<T>());

      return items.Select((x) => new DatabaseObject<T>(x)).ToList();
    }

    public async Task<List<DatabaseObject<T>>> ReadAllAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModel<T>
    {
      List<T> items = await Connection.LoadSelectAsync(predicate);

      return items.Select((x) => new DatabaseObject<T>(x)).ToList();
    }

    public async Task<bool> UpdateAsync<T>(T entity)
      where T : DatabaseModel<T>
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