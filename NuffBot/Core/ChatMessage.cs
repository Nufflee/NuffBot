using System;
using NuffBot.Core;

namespace NuffBot
{
  public class ChatMessage<TUser>
    where TUser : User
  {
    public TUser Sender { get; }
    public string Id { get; }
    public string Content { get; }
    public DateTime TimeStamp { get; }

    protected ChatMessage(TUser sender, string id, string content, DateTime timeStamp)
    {
      Sender = sender;
      Id = id;
      Content = content;
      TimeStamp = timeStamp;
    }
  }
}