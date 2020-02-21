using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuffBot
{
  public interface IDatabaseContext : IDisposable
  {
    Task<bool> WriteAsync<T>(T entity)
      where T : class, IDatabaseObject;

    Task<bool> DeleteAsync<T>(T entity)
      where T : class, IDatabaseObject;

    Task<T> ReadAsync<T>(uint id)
      where T : class, IDatabaseObject;

    Task<List<T>> ReadAllAsync<T>()
      where T : class, IDatabaseObject;

    Task<List<T>> ReadAllAsync<T>(uint id)
      where T : class, IDatabaseObject;
  }
}