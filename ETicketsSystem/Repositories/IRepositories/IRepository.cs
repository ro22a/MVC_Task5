using System.Linq.Expressions;

namespace ETicketsSystem.Repositories.IRepositories
{
	public interface IRepository<T> where T : class
	{
		// CRUD

		Task CreateAsync(T entity);

		void Update(T entity);

		void Delete(T entity);

		Task CommitAsync();

		Task<List<T>> GetAsync(Expression<Func<T, bool>>? expression = null,
			 bool tracked = true, params Expression<Func<T, object>>[] includes);

		Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
			 bool tracked = true, params Expression<Func<T, object>>[] includes);
		//Task GetAsync(object value1, bool v, Func<object, object> includes, Func<object, object> value2);
	}
}
