using Optivio.Domin.Models;
using Optivio.Domin.Models.Enums;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.Services.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileService(IProfileRepository profileRepository, IUnitOfWork unitOfWork)
        {
            _profileRepository = profileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _profileRepository.GetUserWithProfileAsync(userId);
            if (user == null) return null;

            return new ProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.Phones.FirstOrDefault()?.PhoneNumber,
                AvatarUrl = user.Profile?.AvatarUrl,
                DateOfBirth = user.Profile?.DateOfBirth,
                Gender = user.Profile?.Gender.ToString()
            };
        }

        public async Task<ProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _profileRepository.GetUserWithProfileAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            if (user.Profile != null)
            {
                user.Profile.AvatarUrl = dto.AvatarUrl ?? user.Profile.AvatarUrl;
                user.Profile.DateOfBirth = dto.DateOfBirth ?? user.Profile.DateOfBirth;
                if (!string.IsNullOrEmpty(dto.Gender))
                    user.Profile.Gender = Enum.Parse<Gender>(dto.Gender);
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                var phone = user.Phones.FirstOrDefault();
                if (phone != null)
                    phone.PhoneNumber = dto.PhoneNumber;
                else
                    user.Phones.Add(new UserPhone { PhoneNumber = dto.PhoneNumber });
            }

            await _unitOfWork.SaveChangesAsync();

            return new ProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.Phones.FirstOrDefault()?.PhoneNumber,
                AvatarUrl = user.Profile?.AvatarUrl,
                DateOfBirth = user.Profile?.DateOfBirth,
                Gender = user.Profile?.Gender.ToString()
            };
        }

        public async Task<IEnumerable<AddressDto>> GetAddressesAsync(int userId)
        {
            var addresses = await _profileRepository.GetAddressesAsync(userId);
            return addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Country = a.Country,
                City = a.City,
                Street = a.Street,
                BuildingNo = a.BuildingNo
            });
        }

        public async Task<AddressDto> AddAddressAsync(int userId, CreateAddressDto dto)
        {
            var address = new Address
            {
                UserId = userId,
                Country = dto.Country,
                City = dto.City,
                Street = dto.Street,
                BuildingNo = dto.BuildingNo
            };

            await _profileRepository.AddAddressAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return new AddressDto
            {
                Id = address.Id,
                Country = address.Country,
                City = address.City,
                Street = address.Street,
                BuildingNo = address.BuildingNo
            };
        }

        public async Task DeleteAddressAsync(int userId, int addressId)
        {
            var address = await _profileRepository.GetAddressByIdAsync(addressId, userId);
            if (address == null) throw new Exception("Address not found");

            await _profileRepository.RemoveAddressAsync(address);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<AddressDto> UpdateAddressAsync(int userId, int addressId, CreateAddressDto dto)
        {
            var address = await _profileRepository.GetAddressByIdAsync(addressId, userId);
            if (address == null)
                throw new Exception("Address not found");

            address.Country = dto.Country;
            address.City = dto.City;
            address.Street = dto.Street;
            address.BuildingNo = dto.BuildingNo;

            await _profileRepository.UpdateAddressAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return new AddressDto
            {
                Id = address.Id,
                Country = address.Country,
                City = address.City,
                Street = address.Street,
                BuildingNo = address.BuildingNo
            };
        }
    }
}
