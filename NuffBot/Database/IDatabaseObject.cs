using System.Threading.Tasks;

namespace NuffBot
{
  public interface IDatabaseObject
  {
    ulong Id { get; set; }

    Task<bool> SaveToDatabase(IDatabaseContext database);
  }
}