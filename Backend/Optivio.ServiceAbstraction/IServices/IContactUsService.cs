using Optivio.Shared.DTOs.ContactUs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IContactUsService
    {
        Task SubmitAsync(int? userId, CreateContactUsDto dto);
    }
}
