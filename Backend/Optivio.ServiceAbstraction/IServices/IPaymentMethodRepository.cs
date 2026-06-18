using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(int userId);
        Task<PaymentMethod?> GetByIdAsync(int id, int userId);
        Task AddAsync(PaymentMethod paymentMethod);
        Task RemoveAsync(PaymentMethod paymentMethod);
    }
}
