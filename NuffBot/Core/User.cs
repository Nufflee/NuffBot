namespace NuffBot.Core
{
  public class User
  {
    public UserLevel UserLevel { get; }
    public string UserName { get; }
    public ulong Id { get; }

    protected User(UserLevel userLevel, string userName, ulong id)
    {
      UserLevel = userLevel;
      UserName = userName;
      Id = id;
    }
  }
}