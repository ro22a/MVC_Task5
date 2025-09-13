using ETicketsSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace ETicketsSystem.Repositories
{
	
		public class Repository<T> : IRepository<T> where T : class
		{

			private ApplicationDbContext _context;
			private DbSet<T> _db;

			public Repository(ApplicationDbContext context)
			{ 
				_context = context;
				_db = _context.Set<T>();
			}

			// CRUD

			public async Task CreateAsync(T entity)
			{
				await _db.AddAsync(entity);
			}

			public void Update(T entity)
			{
				_db.Update(entity);
			}

			public void Delete(T entity)
			{
				_db.Remove(entity);
			}

			public async Task CommitAsync()
			{
				await _context.SaveChangesAsync();
			}

			public async Task<List<T>> GetAsync(Expression<Func<T, bool>>? expression = null,
				bool tracked = true, params Expression<Func<T, object>>[] includes)
			{
				var entities = _db.AsQueryable();

				if (expression is not null)
				{
					entities = entities.Where(expression);
				}

				if (includes is not null)
				{
					foreach (var item in includes)
					{
						entities = entities.Include(item);
					}
				}

				if (!tracked)
				{
					entities = entities.AsNoTracking();
				}

				return await entities.ToListAsync();
			}

			public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
				 bool tracked = true, params Expression<Func<T, object>>[] includes)
			{
				return (await GetAsync(expression, tracked,includes)).FirstOrDefault();
			}
		}
	
}
