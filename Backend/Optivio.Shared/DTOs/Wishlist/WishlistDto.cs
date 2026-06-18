namespace Optivio.Shared.DTOs.Wishlist
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new();
    }

    public class WishlistItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public DateTime AddedAt { get; set; }
    }
}