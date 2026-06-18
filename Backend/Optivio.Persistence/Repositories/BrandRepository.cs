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
    public class BrandRepository : IBrandRepository
    {
        private readonly ObtivioDbContext _context;

        public BrandRepository(ObtivioDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ProductBrand>> GetAllAsync()
            => await _context.ProductBrands.ToListAsync();

        public async Task<ProductBrand?> GetByIdAsync(int id)
            => await _context.ProductBrands.FirstOrDefaultAsync(b => b.Id == id);

        public async Task AddAsync(ProductBrand brand)
            => await _context.ProductBrands.AddAsync(brand);

        public Task UpdateAsync(ProductBrand brand)
        {
            _context.ProductBrands.Update(brand);
            return Task.CompletedTask;
        }
    }
}
