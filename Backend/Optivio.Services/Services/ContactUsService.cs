using Optivio.Domin.Models;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.ContactUs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Services.Services
{
    public class ContactUsService : IContactUsService
    {
        private readonly IContactUsRepository _contactUsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContactUsService(IContactUsRepository contactUsRepository, IUnitOfWork unitOfWork)
        {
            _contactUsRepository = contactUsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task SubmitAsync(int? userId, CreateContactUsDto dto)
        {
            var contact = new ContactUs
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                OrderNumber = dto.OrderNumber,
                Country = dto.Country,
                Type = dto.Type,
                Reason = dto.Reason,
                Message = dto.Message,
                UserId = userId
            };

            await _contactUsRepository.AddAsync(contact);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
