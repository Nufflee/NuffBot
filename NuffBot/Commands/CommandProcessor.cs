using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NuffBot.Core;
using NuffBot.Discord;

namespace NuffBot.Commands
{
  public static class CommandProcessor
  {
    private static readonly List<Command> staticCommands;

    static CommandProcessor()
    {
      staticCommands = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(Command).IsAssignableFrom(type) && type != typeof(Command)).Select(Activator.CreateInstance).Cast<Command>().ToList();
    }

    public static async Task ProcessCommand<TUser>(ChatMessage<TUser> chatMessage, Bot bot)
      where TUser : User
    {
      if (chatMessage.Sender.Id == bot.Id)
      {
        return;
      }
      
      string message = chatMessage.Content;
      string name = message.TrimStart('!').Split(' ')[0].ToLower();

      Command commandToExecute = staticCommands.FirstOrDefault(command => command.Name == name);

      if (commandToExecute == null)
      {
        commandToExecute = staticCommands.FirstOrDefault(command => command.Aliases.Any(alias => alias == name));
      }

      CommandContext context = new CommandContext(DiscordBot.CurrentGuild, TwitchBot.CurrentChannel);

      if (chatMessage is DiscordChatMessage discordChatMessage)
      {
        context = new DiscordCommandContext(context, discordChatMessage.Channel);
      }
      
      if (commandToExecute == null)
      {
        DatabaseObject<DatabaseCommand> dbCommand = await SqliteDatabase.Instance.ReadSingleAsync<DatabaseCommand>(c => c.Name == name);
        
        if (dbCommand.Entity != null)
        {
          bot.SendMessage(dbCommand.Entity.Response, context);
        }

        return;
      }

      if (chatMessage.Sender.UserLevel < commandToExecute.UserLevel)
      {
        return;
      }

      commandToExecute.DoExecute(chatMessage, bot, context);
    }
  }
}