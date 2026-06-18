using Optivio.Shared.DTOs.ContactUs;
using Optivio.Shared.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IAdminService
    {
        Task<IEnumerable<AdminUserDto>> GetAllUsersAsync();
        Task UpdateUserStatusAsync(int userId, UpdateUserStatusDto dto);
        Task DeleteUserAsync(int userId);
        Task<IEnumerable<ContactUsDto>> GetAllContactMessagesAsync();
    }
}
