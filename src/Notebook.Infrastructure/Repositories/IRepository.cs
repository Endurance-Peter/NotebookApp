using Notebook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Infrastructure.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T> GetById(Guid id);
        Task<bool> Exist(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAll(); 

    }
}
