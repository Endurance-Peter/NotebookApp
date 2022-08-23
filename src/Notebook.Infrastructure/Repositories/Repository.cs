using Microsoft.EntityFrameworkCore;
using Notebook.Infrastructure.Configurations;
using Notebook.Models;
using System.Linq.Expressions;

namespace Notebook.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        
        public readonly DbSet<T> _session;

        public Repository(ApplicationContext session)
        {
            _session = session.Set<T>();
        }
        public void Add(T entity)
        {
            _session.Add(entity);
        }

        public void Delete(T entity)
        {
            _session.Remove(entity);
        }

        public Task<bool> Exist(Expression<Func<T,bool>> expression)
        {
            return Task.FromResult(_session.Any(expression));
        }

        public async Task<List<T>> GetAll()
        {
            return await _session.ToListAsync();
        }

        public async Task<T> GetById(Guid id)
        {
            return await _session.FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(T entity)
        {
            _session.Update(entity);
        }
    }
}
