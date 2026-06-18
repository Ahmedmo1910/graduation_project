using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
        Task<Review?> GetByIdAsync(int reviewId);
        Task<bool> HasUserReviewedProductAsync(int userId, int productId);
        Task AddAsync(Review review);
        Task RemoveAsync(Review review);
        Task UpdateAsync(Review review);
    }
}
