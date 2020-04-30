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
      SqliteDatabase.Connect(Configuration.DatabasePath);

      new Thread(() => { new TwitchBot(); }).Start();

      new Thread(() => { new DiscordBot(); }).Start();

      await Task.Delay(-1);
    }
  }
}