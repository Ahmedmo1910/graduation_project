using Microsoft.EntityFrameworkCore;
using Optivio.Domin.Models;
using Optivio.Domin.Models.Enums;
using Optivio.Persistence.Data.DbContexts;
using Optivio.ServiceAbstraction.IServices;
using Optivio.Shared.DTOs.Product;

namespace Optivio.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ObtivioDbContext _context;

        public ProductRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.ProductBrands)
                .Include(p => p.ProductCategories)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(p => p.Name.Contains(filter.Search));

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            if (filter.BrandId.HasValue)
                query = query.Where(p => p.BrandId == filter.BrandId);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            query = filter.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                _ => query.OrderBy(p => p.Id)
            };

            return await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(ProductFilterDto filter)
        {
            var query = _context.Products.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(p => p.Name.Contains(filter.Search));

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            if (filter.BrandId.HasValue)
                query = query.Where(p => p.BrandId == filter.BrandId);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            return await query.CountAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
            => await _context.Products
                .Include(p => p.ProductBrands)
                .Include(p => p.ProductCategories)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
            => await _context.Products
                .Include(p => p.ProductBrands)
                .Include(p => p.Reviews)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();

        public async Task<IEnumerable<Product>> GetByBrandAsync(int brandId)
            => await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.Reviews)
                .Where(p => p.BrandId == brandId && p.IsActive)
                .ToListAsync();

        public Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }
        public async Task<IEnumerable<Product>> GetSimilarProductsAsync(int productId, int categoryId)
            => await _context.Products
        .Include(p => p.ProductBrands)
        .Include(p => p.ProductCategories)
        .Where(p => p.CategoryId == categoryId && p.Id != productId && p.IsActive)
        .Take(6)
        .ToListAsync();

        public async Task<IEnumerable<Product>> GetByFaceShapeAsync(string faceShape, int pageSize)
        {
            if (!Enum.TryParse<FaceShape>(faceShape, ignoreCase: true, out var shape))
                return Enumerable.Empty<Product>();

            return await _context.Products
                .Include(p => p.ProductBrands)
                .Include(p => p.ProductCategories)
                .Include(p => p.Reviews)
                .Include(p => p.SuitableFaceShapes)
                .Where(p => p.IsActive &&
                            p.SuitableFaceShapes.Any(f => f.FaceShape == shape))
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopRatedAsync(int pageSize)
        {
            return await _context.Products
                .Include(p => p.ProductBrands)
                .Include(p => p.ProductCategories)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}