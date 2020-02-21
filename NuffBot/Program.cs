using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NuffBot.Discord;

namespace NuffBot
{
  public class Program
  {
    private static async Task Main(string[] args)
    {
      DotNetEnv.Env.Load();

      Console.WriteLine(Directory.GetCurrentDirectory());
      
      Console.WriteLine(Environment.GetEnvironmentVariable("TwitchChannelName"));
      
      new Thread(() => { new TwitchBot(); }).Start();

      new Thread(() => { new DiscordBot(); }).Start();

      await Task.Delay(-1);
    }
  }
}