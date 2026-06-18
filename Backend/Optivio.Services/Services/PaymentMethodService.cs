using Optivio.Domin.Models;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Services.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PaymentMethodDto>> GetMyPaymentMethodsAsync(int userId)
        {
            var methods = await _paymentMethodRepository.GetByUserIdAsync(userId);
            return methods.Select(m => new PaymentMethodDto
            {
                Id = m.Id,
                Provider = m.Provider,
                LastDigits = m.LastDigits,
                ExpireDate = m.ExpireDate,
                IsDefault = m.IsDefault
            });
        }

        public async Task<PaymentMethodDto> AddPaymentMethodAsync(int userId, CreatePaymentMethodDto dto)
        {
            var paymentMethod = new PaymentMethod
            {
                UserId = userId,
                Provider = dto.Provider,
                LastDigits = dto.LastDigits,
                ExpireDate = dto.ExpireDate,
                IsDefault = dto.IsDefault
            };

            await _paymentMethodRepository.AddAsync(paymentMethod);
            await _unitOfWork.SaveChangesAsync();

            return new PaymentMethodDto
            {
                Id = paymentMethod.Id,
                Provider = paymentMethod.Provider,
                LastDigits = paymentMethod.LastDigits,
                ExpireDate = paymentMethod.ExpireDate,
                IsDefault = paymentMethod.IsDefault
            };
        }

        public async Task DeletePaymentMethodAsync(int userId, int paymentMethodId)
        {
            var method = await _paymentMethodRepository.GetByIdAsync(paymentMethodId, userId);
            if (method == null)
                throw new Exception("Payment method not found");

            await _paymentMethodRepository.RemoveAsync(method);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
