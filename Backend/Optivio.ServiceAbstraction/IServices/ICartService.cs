using Optivio.Shared.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto);
        Task<CartDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemDto dto);
        Task RemoveFromCartAsync(int userId, int itemId);
        Task ClearCartAsync(int userId);
    }
}
