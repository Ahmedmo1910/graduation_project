using Optivio.Shared.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices
{
    public interface IProfileService
    {
        Task<ProfileDto?> GetProfileAsync(int userId);
        Task<ProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task<IEnumerable<AddressDto>> GetAddressesAsync(int userId);
        Task<AddressDto> AddAddressAsync(int userId, CreateAddressDto dto);
        Task DeleteAddressAsync(int userId, int addressId);
        Task<AddressDto> UpdateAddressAsync(int userId, int addressId, CreateAddressDto dto);
    }
}
