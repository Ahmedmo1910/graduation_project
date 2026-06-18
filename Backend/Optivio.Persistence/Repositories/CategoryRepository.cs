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
    public class CategoryRepository : ICategoryRepository
    {

        private readonly ObtivioDbContext _context;

        public CategoryRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
            => await _context.ProductCategories.ToListAsync();

        public async Task<ProductCategory?> GetByIdAsync(int id)
            => await _context.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(ProductCategory category)
            => await _context.ProductCategories.AddAsync(category);

        public Task UpdateAsync(ProductCategory category)
        {
            _context.ProductCategories.Update(category);
            return Task.CompletedTask;
        }

    }
}
