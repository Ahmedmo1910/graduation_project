using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IWishlistRepository
    {
        Task<Wishlist?> GetByUserIdAsync(int userId);
        Task<Wishlist> CreateAsync(int userId);
        Task<WishlistItem?> GetItemAsync(int wishlistId, int productId);
        Task AddItemAsync(WishlistItem item);
        Task RemoveItemAsync(WishlistItem item);
    }
}
