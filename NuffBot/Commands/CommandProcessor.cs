using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NuffBot.Core;
using NuffBot.Database.Models;
using NuffBot.Discord;

namespace NuffBot.Commands
{
  public static class CommandProcessor
  {
    public static IEnumerable<Command> StaticCommands { get; } = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(Command).IsAssignableFrom(type) && type != typeof(Command)).Select(Activator.CreateInstance).Cast<Command>().ToList();

    public static async Task ProcessCommand<TUser>(ChatMessage<TUser> chatMessage, Bot bot)
      where TUser : User
    {
      if (chatMessage.Sender.Id == bot.Id)
      {
        return;
      }

      string message = chatMessage.Content;
      string name = message.TrimStart('!').Split(' ')[0].ToLower();

      DatabaseObject<CommandModel> commandToExecute = await DatabaseHelper.GetCommandByNameOrAlias(name);

      if (!commandToExecute.Exists())
      {
        return;
      }

      CommandContext context = new CommandContext(DiscordBot.CurrentGuild, TwitchBot.CurrentChannel);

      if (chatMessage is DiscordChatMessage discordChatMessage)
      {
        context = new DiscordCommandContext(context, discordChatMessage.Channel);
      }

      if (commandToExecute.Entity.IsStaticCommand)
      {
        Command staticCommands = StaticCommands.First((c) => c.Name == name);

        if (chatMessage.Sender.UserLevel < staticCommands.UserLevel)
        {
          return;
        }

        staticCommands.DoExecute(chatMessage, bot, context);
      }
      else
      {
        bot.SendMessage(commandToExecute.Entity.Response, context);
      }

      DatabaseObject<CommandMetricsModel> dbMetrics = await DatabaseHelper.GetCommandMetricsByCommand(commandToExecute.Entity);

      if (dbMetrics.Exists())
      {
        dbMetrics.Entity.ExecutionCount++;

        await dbMetrics.UpdateInDatabase(SqliteDatabase.Instance);
      }
      else
      {
        CommandMetricsModel metrics = new CommandMetricsModel(commandToExecute.Entity);

        await metrics.SaveToDatabase(SqliteDatabase.Instance);
      }
    }
  }
}