using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NuffBot
{
  public interface IDatabaseContext : IDisposable
  {
    Task<bool> WriteAsync<T>(T entity)
      where T : DatabaseModelBase<T>;

    Task<bool> DeleteEntityAsync<T>(T entity)
      where T : DatabaseModelBase<T>;

    Task<bool> DeleteAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModelBase<T>;

    Task<DatabaseObject<T>> ReadSingleAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModelBase<T>;

    Task<DatabaseObject<T>> ReadSingleByIdAsync<T>(ulong id)
      where T : DatabaseModelBase<T>;
    
    Task<List<DatabaseObject<T>>> ReadAllAsync<T>()
      where T : DatabaseModelBase<T>;

    Task<List<DatabaseObject<T>>> ReadAllAsync<T>(Expression<Func<T, bool>> predicate)
      where T : DatabaseModelBase<T>;

    Task<bool> UpdateAsync<T>(T entity)
      where T : DatabaseModelBase<T>;
  }
}