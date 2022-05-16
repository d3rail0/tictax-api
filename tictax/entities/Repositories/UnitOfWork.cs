using entities.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {

        private readonly AppDbContext _appDbContext;

        public IUserRepository Users { get; private set; }

        public UnitOfWork(
                AppDbContext appDbContext,
                ILoggerFactory loggerFactory
            )
        {
            _appDbContext = appDbContext;

            Users = new UserRepository(_appDbContext);

        }

        public async Task CompleteAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _appDbContext.DisposeAsync();
        }

        public void Dispose()
        {
            _appDbContext.Dispose();
        }
    }
}
