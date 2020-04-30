using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NuffBot
{
  public interface IDatabaseContext : IDisposable
  {
    Task<bool> WriteAsync<T>(T entity)
      where T : DatabaseModel;

    Task<bool> DeleteEntityAsync<T>(T entity)
      where T : DatabaseModel;

    Task<bool> DeleteAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModel;

    Task<DatabaseObject<T>> ReadSingleAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModel;

    Task<List<DatabaseObject<T>>> ReadAllAsync<T>()
      where T : DatabaseModel;

    Task<List<DatabaseObject<T>>> ReadAllAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModel;

    Task<bool> UpdateAsync<T>(T entity)
      where T : DatabaseModel;
  }
}