using Optivio.Domin.Models;
using Optivio.Domin.Models.Enums;
using Optivio.ServiceAbstraction.IServices;
using Optivio.ServiceAbstraction.IServices.IJwtService.cs;
using Optivio.ServiceAbstraction.IServices.IUserRepository.cs;
using Optivio.ServiceAbstraction.IUnitOfWork;
using Optivio.Shared.DTOs.Auth;

namespace Optivio.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork,
                           IJwtService jwtService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid email or password");

            var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isValid)
                throw new Exception("Invalid email or password");

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Token = _jwtService.GenerateToken(user),
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var exists = await _userRepository.EmailExistsAsync(dto.Email);
            if (exists)
                throw new Exception("Email already exists");

            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Passwords do not match");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = UserRole.Customer,
                Status = Status.Active,
                Profile = new Profile
                {
                    DateOfBirth = DateTime.UtcNow,
                    Gender = Gender.Male,
                    AvatarUrl = ""
                }
            };

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                user.Phones.Add(new UserPhone { PhoneNumber = dto.PhoneNumber });

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Token = _jwtService.GenerateToken(user),
                Role = user.Role.ToString()
            };
        }
        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Email not found");

            var token = Guid.NewGuid().ToString("N");
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _unitOfWork.SaveChangesAsync();

            var resetLink = $"http://localhost:4200/reset-password?token={token}&email={dto.Email}";
            var body = $@"
        <h2>Reset Your Password</h2>
        <p>Click the link below to reset your password:</p>
        <a href='{resetLink}'>Reset Password</a>
        <p>This link expires in 1 hour.</p>
    ";

            await _emailService.SendEmailAsync(dto.Email, "Reset Your Password - Optivio", body);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("Invalid request");

            if (user.ResetPasswordToken != dto.Token)
                throw new Exception("Invalid token");

            if (user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                throw new Exception("Token expired");

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("Passwords do not match");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new Exception("Current password is incorrect");

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("Passwords do not match");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}