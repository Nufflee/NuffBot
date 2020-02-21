using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NuffBot.Core;
using NuffBot.Core.Twitch;
using NuffBot.Discord;

namespace NuffBot.Commands
{
  public static class CommandProcessor
  {
    private static readonly List<Command> commands;

    static CommandProcessor()
    {
      commands = GetAllCommands();
    }

    private static List<Command> GetAllCommands()
    {
      List<Command> allCommands = new List<Command>();

      allCommands.AddRange(Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(Command).IsAssignableFrom(type) && type != typeof(Command)).Select(Activator.CreateInstance).Cast<Command>().ToList());

      return allCommands;
    }

    public static void ProcessCommand<TUser>(ChatMessage<TUser> chatMessage, Bot bot)
      where TUser : User
    {
      if (chatMessage.Sender.Id == bot.Id)
      {
        return;
      }
      
      string message = chatMessage.Content;
      string name = message.TrimStart('!').Split(' ')[0].ToLower();

      Command commandToExecute = commands.FirstOrDefault(command => command.Name == name);

      if (commandToExecute == null)
      {
        commandToExecute = commands.FirstOrDefault(command => command.Aliases.Any(alias => alias == name));
      }

      if (commandToExecute == null)
      {
        return;
      }

      if (chatMessage.Sender.UserLevel < commandToExecute.UserLevel)
      {
        return;
      }

      CommandContext context = new CommandContext(DiscordBot.CurrentGuild, TwitchBot.CurrentChannel);

      if (chatMessage is DiscordChatMessage discordChatMessage)
      {
        context = new DiscordCommandContext(context, discordChatMessage.Channel);
      }

      commandToExecute.DoExecute(chatMessage, bot, context);
    }
  }
}