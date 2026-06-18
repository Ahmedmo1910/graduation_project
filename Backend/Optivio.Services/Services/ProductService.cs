using Optivio.Domin.Models;
using Optivio.Domin.Models.Enums;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Product;

namespace Optivio.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResultDto<ProductDto>> GetAllAsync(ProductFilterDto filter)
        {
            var products = await _productRepository.GetAllAsync(filter);
            var total = await _productRepository.GetCountAsync(filter);
            return new PagedResultDto<ProductDto>
            {
                Data = products.Select(MapToDto),
                TotalCount = total,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> GetByBrandAsync(int brandId)
        {
            var products = await _productRepository.GetByBrandAsync(brandId);
            return products.Select(MapToDto);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Color = dto.Color,
                Gender = Enum.Parse<Gender>(dto.Gender),
                Size = dto.Size,
                LensType = Enum.Parse<LensType>(dto.LensType),
                Price = dto.Price,
                Currency = dto.Currency,
                StockQuantity = dto.StockQuantity,
                ThumbnailUrl = dto.ThumbnailUrl,
                MediaUrl = dto.MediaUrl,
                BrandId = dto.BrandId,
                CategoryId = dto.CategoryId,
                IsActive = true
            };

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Color = product.Color,
                Gender = product.Gender.ToString(),
                Size = product.Size,
                LensType = product.LensType.ToString(),
                Price = product.Price,
                Currency = product.Currency,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                ThumbnailUrl = product.ThumbnailUrl,
                MediaUrl = product.MediaUrl,
                TwoDImageUrl = product.ThumbnailUrl != null
                    ? $"https://backendgraduationproject1.runasp.net/api/files/images2d/{Path.GetFileName(product.ThumbnailUrl)}"
                    : null
            };
        }

        private ProductDto MapToDto(Product p) => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Color = p.Color,
            Gender = p.Gender.ToString(),
            Size = p.Size,
            LensType = p.LensType.ToString(),
            Price = p.Price,
            Currency = p.Currency,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            ThumbnailUrl = p.ThumbnailUrl,
            MediaUrl = p.MediaUrl,
            BrandName = p.ProductBrands?.Name ?? "",
            CategoryName = p.ProductCategories?.Name ?? "",
            AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
            TwoDImageUrl = p.ThumbnailUrl != null
                ? $"https://backendgraduationproject1.runasp.net/api/files/images2d/{Path.GetFileName(p.ThumbnailUrl)}"
                : null
        };

        public async Task<ProductDto> UpdateAsync(int id, CreateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Color = dto.Color;
            product.Gender = Enum.Parse<Gender>(dto.Gender);
            product.Size = dto.Size;
            product.LensType = Enum.Parse<LensType>(dto.LensType);
            product.Price = dto.Price;
            product.Currency = dto.Currency;
            product.StockQuantity = dto.StockQuantity;
            product.ThumbnailUrl = dto.ThumbnailUrl;
            product.MediaUrl = dto.MediaUrl;
            product.BrandId = dto.BrandId;
            product.CategoryId = dto.CategoryId;

            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Color = product.Color,
                Gender = product.Gender.ToString(),
                Size = product.Size,
                LensType = product.LensType.ToString(),
                Price = product.Price,
                Currency = product.Currency,
                StockQuantity = product.StockQuantity,
                ThumbnailUrl = product.ThumbnailUrl,
                MediaUrl = product.MediaUrl,
                BrandId = product.BrandId,
                CategoryId = product.CategoryId,
                TwoDImageUrl = product.ThumbnailUrl != null
                    ? $"https://backendgraduationproject1.runasp.net/api/files/images2d/{Path.GetFileName(product.ThumbnailUrl)}"
                    : null
            };
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            product.IsActive = false;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetSimilarProductsAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            var similar = await _productRepository.GetSimilarProductsAsync(productId, product.CategoryId);
            return similar.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Color = p.Color,
                Gender = p.Gender.ToString(),
                Size = p.Size,
                LensType = p.LensType.ToString(),
                Price = p.Price,
                Currency = p.Currency,
                StockQuantity = p.StockQuantity,
                ThumbnailUrl = p.ThumbnailUrl,
                MediaUrl = p.MediaUrl,
                BrandName = p.ProductBrands?.Name ?? "",
                CategoryName = p.ProductCategories?.Name ?? "",
                IsActive = p.IsActive,
                AverageRating = 0,
                TwoDImageUrl = p.ThumbnailUrl != null
                    ? $"https://backendgraduationproject1.runasp.net/api/files/images2d/{Path.GetFileName(p.ThumbnailUrl)}"
                    : null
            });
        }
    }
}