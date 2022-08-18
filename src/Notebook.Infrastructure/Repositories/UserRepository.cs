using Notebook.Infrastructure.Configurations;
using Notebook.Models.Users;

namespace Notebook.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext session) : base(session)
        {

        }
    }
}
