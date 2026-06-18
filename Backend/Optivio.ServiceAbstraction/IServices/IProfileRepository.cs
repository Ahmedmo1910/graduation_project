using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IProfileRepository
    {
        Task<User?> GetUserWithProfileAsync(int userId);
        Task<IEnumerable<Address>> GetAddressesAsync(int userId);
        Task<Address?> GetAddressByIdAsync(int addressId, int userId);
        Task AddAddressAsync(Address address);
        Task RemoveAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
    }
}
