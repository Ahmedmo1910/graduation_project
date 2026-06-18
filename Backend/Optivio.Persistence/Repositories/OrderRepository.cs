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
    public class OrderRepository : IOrderRepository
    {
        private readonly ObtivioDbContext _context;

        public OrderRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
           => await _context.Orders
               .Include(o => o.OrderItems)
               .ThenInclude(i => i.Product)
               .Where(o => o.UserId == userId)
               .OrderByDescending(o => o.OrderDate)
               .ToListAsync();

        public async Task<Order?> GetByIdAsync(int orderId, int userId)
            => await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        public async Task AddAsync(Order order)
            => await _context.Orders.AddAsync(order);
        public async Task<IEnumerable<Order>> GetAllAsync()
    => await _context.Orders
        .Include(o => o.OrderItems)
        .ThenInclude(i => i.Product)
        .Include(o => o.User)
        .OrderByDescending(o => o.OrderDate)
        .ToListAsync();

        public async Task<Order?> GetByIdForAdminAsync(int orderId)
            => await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
    }
}
