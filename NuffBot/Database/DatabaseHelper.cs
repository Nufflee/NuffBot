using System.Collections.Generic;
using System.Linq;
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

    public static async Task<List<DatabaseObject<CommandModel>>> GetAllCommands()
    {
      return await SqliteDatabase.Instance.ReadAllAsync<CommandModel>();
    }

    public static async Task<DatabaseObject<CommandModel>> GetCommandByAlias(string alias)
    {
      List<DatabaseObject<CommandModel>> allCommands = await GetAllCommands();

      return new DatabaseObject<CommandModel>(allCommands.Select((c) => c.Entity).FirstOrDefault(c => c.Aliases.Contains(alias)));
    }

    public static async Task<DatabaseObject<TimerModel>> GetTimerByCommand(CommandModel command)
    {
      return await SqliteDatabase.Instance.ReadSingleAsync<TimerModel>((t) => t.CommandId == command.Id);
    }

    public static async Task<List<DatabaseObject<TimerModel>>> GetAllTimers()
    {
      return await SqliteDatabase.Instance.ReadAllAsync<TimerModel>();
    }

    public static async Task<DatabaseObject<CommandModel>> GetCommandById(ulong id)
    {
      return await SqliteDatabase.Instance.ReadSingleAsync<CommandModel>((c) => c.Id == id);
    }
  }
}