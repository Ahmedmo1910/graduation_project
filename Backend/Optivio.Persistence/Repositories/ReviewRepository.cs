using Microsoft.EntityFrameworkCore;
using Optivio.Domin.Models;
using Optivio.Persistence.Data.DbContexts;
using Optivio.ServiceAbstraction.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Persistence.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ObtivioDbContext _context;

        public ReviewRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
            => await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

        public async Task<Review?> GetByIdAsync(int reviewId)
            => await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);

        public async Task<bool> HasUserReviewedProductAsync(int userId, int productId)
            => await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId);

        public async Task AddAsync(Review review)
            => await _context.Reviews.AddAsync(review);

        public Task RemoveAsync(Review review)
        {
            _context.Reviews.Remove(review);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            return Task.CompletedTask;
        }
    }
}

