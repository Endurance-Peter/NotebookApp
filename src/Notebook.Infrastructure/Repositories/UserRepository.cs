using Notebook.Infrastructure.Configurations;
using Notebook.Models.Users;
using System.Linq.Expressions;

namespace Notebook.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext session) : base(session)
        {

        }

        public Task<User> GetUserAsync(Expression<Func<User, bool>> expression)
        {
            return Task.FromResult(_session.FirstOrDefault(expression));
        }
    }
}
