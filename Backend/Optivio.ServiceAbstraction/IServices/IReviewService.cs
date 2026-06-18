using Optivio.Shared.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId);
        Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto);
        Task DeleteReviewAsync(int userId, int reviewId);
        Task<ReviewDto> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto);
    }
}
