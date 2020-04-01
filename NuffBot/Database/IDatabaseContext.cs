using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NuffBot
{
  public interface IDatabaseContext : IDisposable
  {
    Task<bool> WriteAsync<T>(T entity)
      where T : class, IDatabaseObject;

    Task<bool> DeleteEntityAsync<T>(T entity)
      where T : class, IDatabaseObject;

    Task<bool> DeleteAsync<T>(Expression<Func<T, bool>> predicate)
      where T : class, IDatabaseObject;

    Task<DatabaseObject<T>> ReadSingleAsync<T>(Expression<Func<T, bool>> predicate)
      where T : class, IDatabaseObject;

    Task<List<DatabaseObject<T>>> ReadAllAsync<T>()
      where T : class, IDatabaseObject;

    Task<List<DatabaseObject<T>>> ReadAllAsync<T>(Expression<Func<T, bool>> predicate)
      where T : class, IDatabaseObject;

    Task<bool> UpdateAsync<T>(T entity)
      where T : class, IDatabaseObject;
  }
}