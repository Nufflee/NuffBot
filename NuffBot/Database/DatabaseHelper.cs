using System.Collections.Generic;
using System.Threading.Tasks;
using NuffBot.Commands;
using NuffBot.Core;

namespace NuffBot
{
  public static class DatabaseHelper
  {
    public static async Task<DatabaseObject<CommandModel>> GetCommandByNameOrAlias(string name)
    {
      DatabaseObject<CommandModel> dbCommand = await SqliteDatabase.Instance.ReadSingleAsync<CommandModel>(c => c.Name == name);

      if (!dbCommand.Exists())
      {
        dbCommand = await GetCommandByAlias(name);
      }

      return dbCommand;
    }

    public static Task<List<DatabaseObject<CommandModel>>> GetAllCommands()
    {
      return SqliteDatabase.Instance.ReadAllAsync<CommandModel>();
    }

    public static async Task<DatabaseObject<CommandModel>> GetCommandByAlias(string alias)
    {
      DatabaseObject<AliasModel> dbAlias = await GetAliasByName(alias);

      if (dbAlias.Exists())
      {
        return await SqliteDatabase.Instance.ReadSingleByIdAsync<CommandModel>(dbAlias.Entity.CommandId);
      }

      return DatabaseObject<CommandModel>.Empty;
    }

    public static Task<DatabaseObject<AliasModel>> GetAliasByName(string alias)
    {
      return SqliteDatabase.Instance.ReadSingleAsync<AliasModel>((a) => a.Alias == alias);
    }

    public static Task<List<DatabaseObject<AliasModel>>> GetAllAliasesOfCommand(CommandModel command)
    {
      return SqliteDatabase.Instance.ReadAllAsync<AliasModel>((a) => a.CommandId == command.Id);
    }

    public static Task<DatabaseObject<TimerModel>> GetTimerByCommand(CommandModel command)
    {
      return SqliteDatabase.Instance.ReadSingleAsync<TimerModel>((t) => t.CommandId == command.Id);
    }

    public static Task<List<DatabaseObject<TimerModel>>> GetAllTimers()
    {
      return SqliteDatabase.Instance.ReadAllAsync<TimerModel>();
    }

    public static Task<DatabaseObject<CommandModel>> GetCommandById(ulong id)
    {
      return SqliteDatabase.Instance.ReadSingleByIdAsync<CommandModel>(id);
    }
  }
}