using System;
using System.Diagnostics;
using NuffBot.Commands;
using NuffBot.Core;
using NuffBot.Core.Twitch;
using ServiceStack;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace NuffBot
{
  public class TwitchBot : Bot
  {
    public override ulong Id { get; protected set; }
    public static Channel CurrentChannel { get; private set; }
    public static TwitchAPI TwitchApi { get; private set; }

    private readonly TwitchClient client;
    private readonly ConnectionCredentials credentials = new ConnectionCredentials(Configuration.TwitchBotUsername, Configuration.TwitchBotOAuth);

    public TwitchBot()
    {
      client = new TwitchClient();

      client.Initialize(credentials, Configuration.TwitchChannelName);

      TwitchApi = new TwitchAPI();

      TwitchApi.Settings.ClientId = Configuration.TwitchClientId;
      TwitchApi.Settings.AccessToken = Configuration.TwitchBotOAuth;

      client.OnJoinedChannel += OnJoinedChannel;
      client.OnMessageReceived += OnMessageReceived;

      client.Connect();

      TwitchApi.Settings.SkipDynamicScopeValidation = true;

      Console.WriteLine("Twitch bot connected!");
    }

    public override void SendMessage(string message, CommandContext context)
    {
      client.SendMessage(context.TwitchChannel.Name, message);
    }

    public override void SendPrivateMessage(User receiver, string message, CommandContext context)
    {
      client.SendWhisper(receiver.UserName, message);
    }

    private void OnMessageReceived(object sender, OnMessageReceivedArgs args)
    {
      Stopwatch sw = Stopwatch.StartNew();

      ChatMessage message = args.ChatMessage;

      if (message.Message.StartsWith("!"))
      {
        CommandProcessor.ProcessCommand(new TwitchChatMessage(message), this);
      }

      Console.WriteLine(sw.ElapsedMilliseconds);
    }

    private void OnJoinedChannel(object sender, OnJoinedChannelArgs args)
    {
      CurrentChannel = TwitchApi.V5.Channels.GetChannelByIDAsync(TwitchUser.GetByName(Configuration.TwitchChannelName).Id.ToString()).Result;
      Id = TwitchUser.GetByName(Configuration.TwitchBotUsername).Id;
      //SendMessage("Yo, yo! I'm back!");
    }
  }
}