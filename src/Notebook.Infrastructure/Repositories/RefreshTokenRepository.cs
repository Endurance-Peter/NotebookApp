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

        public Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var result = _session.Where(x => x.Token == refreshToken).FirstOrDefault();
            if (result == null) return null;
            return Task.FromResult(result);
        }

        public Task<bool> MarkRefreshToken(RefreshToken refreshToken)
        {
            var result = _session.FirstOrDefault(x => x.Token == refreshToken.Token);

            if (result == null) return Task.FromResult(false);
            result.IsUsed=refreshToken.IsUsed;

            return Task.FromResult(true);
        }
    }

    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken> GetRefreshToken(string refreshToken);
        Task<bool> MarkRefreshToken(RefreshToken refreshToken);
    }
}
