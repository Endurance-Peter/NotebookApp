using Notebook.Infrastructure.Configurations;
using Notebook.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Infrastructure.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationContext session) : base(session) { }
    }

    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
    }
}
