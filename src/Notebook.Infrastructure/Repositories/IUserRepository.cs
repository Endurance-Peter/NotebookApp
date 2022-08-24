using Notebook.Models.Users;
using System.Linq.Expressions;

namespace Notebook.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserAsync(Expression<Func<User,bool>> expression);
    }
}
