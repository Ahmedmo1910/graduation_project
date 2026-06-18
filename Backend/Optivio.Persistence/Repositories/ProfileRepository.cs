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
    public class ProfileRepository : IProfileRepository
    {

        private readonly ObtivioDbContext _context;

        public ProfileRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserWithProfileAsync(int userId)
           => await _context.Users
               .Include(u => u.Profile)
               .Include(u => u.Phones)
               .FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<IEnumerable<Address>> GetAddressesAsync(int userId)
           => await _context.Addresses
               .Where(a => a.UserId == userId)
               .ToListAsync();

        public async Task<Address?> GetAddressByIdAsync(int addressId, int userId)
            => await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        public async Task AddAddressAsync(Address address)
           => await _context.Addresses.AddAsync(address);

        public Task RemoveAddressAsync(Address address)
        {
            _context.Addresses.Remove(address);
            return Task.CompletedTask;
        }
        public Task UpdateAddressAsync(Address address)
        {
            _context.Addresses.Update(address);
            return Task.CompletedTask;
        }
    }
}
