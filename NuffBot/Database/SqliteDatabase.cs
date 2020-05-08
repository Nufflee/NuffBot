using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NuffBot.Commands;
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

      SqliteOrmLiteDialectProvider.Instance.NamingStrategy = new RemoveSuffixNamingStrategy("Model");

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

      foreach (Type tableType in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsAssignableFromGeneric(typeof(DatabaseModelBase<>)) && t != typeof(DatabaseModelBase<>)).ToArray())
      {
        Connection.CreateTableIfNotExists(tableType);
      }
    }

    public static SqliteDatabase Connect(string path)
    {
      Instance = new SqliteDatabase(path);

      AddStaticCommandsToDatabase();

      return Instance;
    }

    private static async void AddStaticCommandsToDatabase()
    {
      foreach (Command command in CommandProcessor.StaticCommands)
      {
        DatabaseObject<CommandModel> dbCommand = await DatabaseHelper.GetCommandByName(command.Name);

        if (!dbCommand.Exists())
        {
          CommandModel model = new CommandModel(command.Name, "") {IsStaticCommand = true};

          if (!await model.SaveToDatabase(Instance))
          {
            // TODO: Log error

            continue;
          }

          foreach (string alias in command.Aliases)
          {
            if (!await new AliasModel(model, alias).SaveToDatabase(Instance))
            {
              // TODO: Log error

              break;
            }
          }
        }
        else if (!dbCommand.Entity.IsStaticCommand)
        {
          // TODO: Log error: static and dynamic command name clash.
        }
      }
    }

    public Task<bool> WriteAsync<T>(T entity)
      where T : DatabaseModelBase<T>
    {
      return Connection.SaveAsync(entity, true);
    }

    public async Task<bool> DeleteEntityAsync<T>(T entity)
      where T : DatabaseModelBase<T>
    {
      // This may need to be != 0.
      return await Connection.DeleteAsync(entity) > 0;
    }

    public async Task<bool> DeleteAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModelBase<T>
    {
      // This may need to be != 0.
      return await Connection.DeleteAsync(predicate) > 0;
    }

    public async Task<DatabaseObject<T>> ReadSingleAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModelBase<T>
    {
      T item = await Connection.SingleAsync(predicate);

      return new DatabaseObject<T>(item);
    }

    public async Task<DatabaseObject<T>> ReadSingleByIdAsync<T>(ulong id)
      where T : DatabaseModelBase<T>
    {
      T item = await Connection.SingleByIdAsync<T>(id);

      return new DatabaseObject<T>(item);
    }

    public async Task<List<DatabaseObject<T>>> ReadAllAsync<T>()
      where T : DatabaseModelBase<T>
    {
      List<T> items = await Connection.LoadSelectAsync(Connection.From<T>());

      return items.Select((x) => new DatabaseObject<T>(x)).ToList();
    }

    public async Task<List<DatabaseObject<T>>> ReadAllAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModelBase<T>
    {
      List<T> items = await Connection.LoadSelectAsync(predicate);

      return items.Select((x) => new DatabaseObject<T>(x)).ToList();
    }

    public async Task<bool> UpdateAsync<T>(T entity)
      where T : DatabaseModelBase<T>
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