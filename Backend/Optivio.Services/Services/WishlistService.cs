using Optivio.Domin.Models;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Wishlist;

namespace Optivio.Services.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WishlistService(IWishlistRepository wishlistRepository, IUnitOfWork unitOfWork)
        {
            _wishlistRepository = wishlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<WishlistDto> GetWishlistAsync(int userId)
        {
            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = await _wishlistRepository.CreateAsync(userId);
                await _unitOfWork.SaveChangesAsync();
            }

            return new WishlistDto
            {
                Id = wishlist.Id,
                CreatedAt = wishlist.CreatedAt,
                Items = wishlist.Items.Select(i => new WishlistItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    Price = i.Product?.Price ?? 0,
                    ThumbnailUrl = i.Product?.ThumbnailUrl ?? "",
                    AddedAt = i.AddedAt
                }).ToList()
            };
        }

        public async Task AddToWishlistAsync(int userId, int productId)
        {
            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = await _wishlistRepository.CreateAsync(userId);
                await _unitOfWork.SaveChangesAsync();
            }

            var existing = await _wishlistRepository.GetItemAsync(wishlist.Id, productId);
            if (existing != null)
                throw new Exception("Product already in wishlist");

            await _wishlistRepository.AddItemAsync(new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(int userId, int productId)
        {
            var wishlist = await _wishlistRepository.GetByUserIdAsync(userId);
            if (wishlist == null)
                throw new Exception("Wishlist not found");

            var item = await _wishlistRepository.GetItemAsync(wishlist.Id, productId);
            if (item == null)
                throw new Exception("Product not in wishlist");

            await _wishlistRepository.RemoveItemAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}