using System.Linq.Expressions;
using DistributedBanking.Data.Models;

namespace DistributedBanking.Data.Repositories;

public interface IRepositoryBase<T> where T : BaseEntity
{
    Task CreateAsync(T entity);
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
    Task<T> GetAsync(Guid id);
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null);
    Task RemoveAsync(Guid id);
    Task UpdateAsync(T entity);
}