using ServiceStack.OrmLite;

namespace NuffBot
{
  public class RemoveSuffixNamingStrategy : OrmLiteNamingStrategyBase
  {
    private readonly string suffix;
    
    public RemoveSuffixNamingStrategy(string suffix)
    {
      this.suffix = suffix;
    }
    
    public override string GetTableName(string name)
    {
      return name.EndsWith(suffix) ? name[..^suffix.Length] : name;
    }
  }
}