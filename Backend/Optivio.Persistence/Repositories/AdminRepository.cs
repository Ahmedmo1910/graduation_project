using Microsoft.EntityFrameworkCore;
using Optivio.Domin.Models;
using Optivio.Persistence.Data.DbContexts;
using Optivio.ServiceAbstraction.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Persistence.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ObtivioDbContext _context;

        public AdminRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
            => await _context.Users.ToListAsync();

        public async Task<User?> GetUserByIdAsync(int userId)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public Task UpdateUserStatusAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ContactUs>> GetAllContactMessagesAsync()
            => await _context.ContactUs.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }
}
