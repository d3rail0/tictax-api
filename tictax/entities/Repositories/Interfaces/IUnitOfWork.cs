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

        Task CompleteAsync();
    }
}
