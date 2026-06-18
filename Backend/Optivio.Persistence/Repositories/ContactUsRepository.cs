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
    public class ContactUsRepository : IContactUsRepository
    {
        private readonly ObtivioDbContext _context;

        public ContactUsRepository(ObtivioDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ContactUs contactUs)
            => await _context.ContactUs.AddAsync(contactUs);
    }
}
