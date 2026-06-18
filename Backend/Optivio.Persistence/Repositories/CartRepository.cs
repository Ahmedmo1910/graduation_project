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
    public class CartRepository : ICartRepository
    {
        private readonly ObtivioDbContext _context;

        public CartRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Cart cart)
            => await _context.Carts.AddAsync(cart);

        public async Task<Cart?> GetByUserIdAsync(int userId)
            => await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

        public Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            return Task.CompletedTask;
        }
    }
}
