using Optivio.Domin.Models.Enums;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.ContactUs;
using Optivio.Shared.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Services.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
        {
            _adminRepository = adminRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _adminRepository.GetAllUsersAsync();
            return users.Select(u => new AdminUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role.ToString(),
                Status = u.Status.ToString()
            });
        }
        public async Task UpdateUserStatusAsync(int userId, UpdateUserStatusDto dto)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.Status = Enum.Parse<Status>(dto.Status);
            await _adminRepository.UpdateUserStatusAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            await _adminRepository.DeleteUserAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactUsDto>> GetAllContactMessagesAsync()
        {
            var messages = await _adminRepository.GetAllContactMessagesAsync();
            return messages.Select(m => new ContactUsDto
            {
                Id = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                Message = m.Message,
                OrderNumber = m.OrderNumber,
                Country = m.Country,
                Type = m.Type,
                Reason = m.Reason,
                CreatedAt = m.CreatedAt
            });
        }

    }
}
