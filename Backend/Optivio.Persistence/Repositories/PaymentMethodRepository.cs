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
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly ObtivioDbContext _context;

        public PaymentMethodRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(int userId)
            => await _context.PaymentMethods
                .Where(p => p.UserId == userId)
                .ToListAsync();

        public async Task<PaymentMethod?> GetByIdAsync(int id, int userId)
            => await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        public async Task AddAsync(PaymentMethod paymentMethod)
            => await _context.PaymentMethods.AddAsync(paymentMethod);

        public Task RemoveAsync(PaymentMethod paymentMethod)
        {
            _context.PaymentMethods.Remove(paymentMethod);
            return Task.CompletedTask;
        }
    }
}
