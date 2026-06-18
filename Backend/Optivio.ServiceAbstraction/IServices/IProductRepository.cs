using Optivio.Domin.Models;
using Optivio.Shared.DTOs.Product;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(ProductFilterDto filter);
        Task<int> GetCountAsync(ProductFilterDto filter);
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetByBrandAsync(int brandId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<IEnumerable<Product>> GetSimilarProductsAsync(int productId, int categoryId);
        Task<IEnumerable<Product>> GetByFaceShapeAsync(string faceShape, int pageSize);
        Task<IEnumerable<Product>> GetTopRatedAsync(int pageSize);
    }
}