using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuffBot.Commands;
using NuffBot.Core;
using NuffBot.Database.Models;

namespace NuffBot
{
  public static class DatabaseHelper
  {
    public static Task<DatabaseObject<CommandModel>> GetCommandByName(string name)
    {
      return SqliteDatabase.Instance.ReadSingleAsync<CommandModel>(c => c.Name == name);
    }

    public static async Task<DatabaseObject<CommandModel>> GetCommandByNameOrAlias(string name)
    {
      DatabaseObject<CommandModel> dbCommand = await GetCommandByName(name);

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

    public static async Task<DatabaseObject<CommandMetricsModel>> GetCommandMetricsByCommand(CommandModel command)
    {
      List<DatabaseObject<CommandMetricsModel>> metrics = await SqliteDatabase.Instance.ReadAllAsync<CommandMetricsModel>((m) => m.CommandId == command.Id);

      return metrics.FirstOrDefault((m) => m.Entity.Date.AddHours(1) >= DateTime.UtcNow) ?? DatabaseObject<CommandMetricsModel>.Empty;
    }
  }
}