using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IAdminRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task UpdateUserStatusAsync(User user);
        Task DeleteUserAsync(User user);
        Task<IEnumerable<ContactUs>> GetAllContactMessagesAsync();
    }
}
