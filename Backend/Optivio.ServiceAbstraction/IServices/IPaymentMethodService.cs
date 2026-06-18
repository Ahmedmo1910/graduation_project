using Optivio.Shared.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<PaymentMethodDto>> GetMyPaymentMethodsAsync(int userId);
        Task<PaymentMethodDto> AddPaymentMethodAsync(int userId, CreatePaymentMethodDto dto);
        Task DeletePaymentMethodAsync(int userId, int paymentMethodId);
    }
}
