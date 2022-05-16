using entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {

        public Task<User> GetByUsernameAsync(string username);

    }
}
