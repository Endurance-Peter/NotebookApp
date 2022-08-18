using Notebook.Infrastructure.Configurations;
using Notebook.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Infrastructure.UnitOfWorks
{
    public interface IUnitOfWork: IDisposable
    {
        IUserRepository UserRepository { get; set; }
        Task Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _appDbContext;

        public UnitOfWork(ApplicationContext appDbContext)
        {
            _appDbContext = appDbContext;
            InitializeRpositories(appDbContext);
        }

        private void InitializeRpositories(ApplicationContext appDbContext)
        {
            UserRepository = new UserRepository(appDbContext);
        }
        public IUserRepository? UserRepository { get; set; }
        public async Task Commit()
        {
            await _appDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _appDbContext.Dispose();
        }
    }
}
