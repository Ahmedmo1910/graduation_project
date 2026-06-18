using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IBrandRepository
    {
        Task<IEnumerable<ProductBrand>> GetAllAsync();
        Task<ProductBrand?> GetByIdAsync(int id);
        Task AddAsync(ProductBrand brand);
        Task UpdateAsync(ProductBrand brand);
    }
}
