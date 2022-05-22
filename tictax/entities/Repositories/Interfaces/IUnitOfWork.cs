using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        public IUserRepository Users { get; }

        public IMatchRepository Matches { get; }

        public IProfileActivityRepository ProfileActivites { get; }

        Task CompleteAsync();
    }
}
