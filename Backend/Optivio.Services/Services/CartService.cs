using Optivio.Domin.Models;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Services.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                return new CartDto();

            return MapToDto(cart);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.SaveChangesAsync();

            var updatedCart = await _cartRepository.GetByUserIdAsync(userId);
            return MapToDto(updatedCart!);
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, int itemId, UpdateCartItemDto dto)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                throw new Exception("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new Exception("Item not found");

            item.Quantity = dto.Quantity;

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.SaveChangesAsync();

            var updatedCart = await _cartRepository.GetByUserIdAsync(userId);
            return MapToDto(updatedCart!);
        }

        public async Task RemoveFromCartAsync(int userId, int itemId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                throw new Exception("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new Exception("Item not found");

            cart.Items.Remove(item);

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                throw new Exception("Cart not found");

            cart.Items.Clear();

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        private CartDto MapToDto(Cart cart) => new CartDto
        {
            Id = cart.Id,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                ThumbnailUrl = i.Product?.ThumbnailUrl ?? "",
                Price = i.Product?.Price ?? 0,
                Currency = i.Product?.Currency ?? "",
                Quantity = i.Quantity
            })
        };
    }
}
