using Microsoft.EntityFrameworkCore;
using Optivio.Domin.Models;
using Optivio.Persistence.Data.DbContexts;
using Optivio.ServiceAbstraction.IServices.IUserRepository.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ObtivioDbContext _context;
        public UserRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(User user)
        {
            _context.Users.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> EmailExistsAsync(string email)
        => _context.Users.AnyAsync(u => u.Email == email);

        public Task<User?> GetByEmailAsync(string email)
         => _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(int userId)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

}
