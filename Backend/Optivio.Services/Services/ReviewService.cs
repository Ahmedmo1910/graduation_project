using Optivio.Domin.Models;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Title = r.Title,
                Body = r.Body,
                UserName = $"{r.User.FirstName} {r.User.LastName}",
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<ReviewDto> CreateReviewAsync(int userId, CreateReviewDto dto)
        {
            var alreadyReviewed = await _reviewRepository.HasUserReviewedProductAsync(userId, dto.ProductId);
            if (alreadyReviewed)
                throw new Exception("You have already reviewed this product");

            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                OrderId = dto.OrderId,
                Rating = dto.Rating,
                Title = dto.Title,
                Body = dto.Body,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Title = review.Title,
                Body = review.Body,
                UserName = "",
                CreatedAt = review.CreatedAt
            };
        }

        public async Task DeleteReviewAsync(int userId, int reviewId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new Exception("Review not found");

            if (review.UserId != userId)
                throw new Exception("Unauthorized");

            await _reviewRepository.RemoveAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<ReviewDto> UpdateReviewAsync(int userId, int reviewId, UpdateReviewDto dto)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new Exception("Review not found");

            if (review.UserId != userId)
                throw new Exception("Unauthorized");

            review.Rating = dto.Rating;
            review.Title = dto.Title;
            review.Body = dto.Body;
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepository.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Title = review.Title,
                Body = review.Body,
                UserName = "",
                CreatedAt = review.CreatedAt
            };
        }
    }
}
