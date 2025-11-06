using HrApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HrApp.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User> RegisterUserAsync(User user, string password);
        Task<User?> LoginUserAsync(string email, string password);
    }
}
