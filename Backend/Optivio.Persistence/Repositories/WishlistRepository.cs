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
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ObtivioDbContext _context;

        public WishlistRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<Wishlist?> GetByUserIdAsync(int userId)
            => await _context.Wishlists
                .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);

        public async Task<Wishlist> CreateAsync(int userId)
        {
            var wishlist = new Wishlist { UserId = userId };
            await _context.Wishlists.AddAsync(wishlist);
            return wishlist;
        }

        public async Task<WishlistItem?> GetItemAsync(int wishlistId, int productId)
            => await _context.WishlistItems
                .FirstOrDefaultAsync(i => i.WishlistId == wishlistId && i.ProductId == productId);

        public async Task AddItemAsync(WishlistItem item)
            => await _context.WishlistItems.AddAsync(item);

        public Task RemoveItemAsync(WishlistItem item)
        {
            _context.WishlistItems.Remove(item);
            return Task.CompletedTask;
        }
    }
}
