namespace Optivio.Shared.DTOs.Product
{
    public class CreateProductDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string LensType { get; set; } = null!;
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}