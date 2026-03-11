using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace ECommerceAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

            if (user == null)
                return null;

            // Verify password
            //if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            //    return null;
            // Simple password verification (In production, use proper hashing like BCrypt)
            // For demo purposes, password is same as username + "123"
            var expectedPassword = user.Username + "123";
            if (loginDto.Password != expectedPassword)
                return null;

            var token = _jwtService.GenerateToken(user);
            var response = _mapper.Map<LoginResponseDto>(user);
            response.Token = token;

            return response;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username exists
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            {
                          return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Username already exists"
                };
            }

            // Check if email exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = "Email already exists"
                };
            }

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                FullName = registerDto.FullName,
                IsAdmin = false,
                CreatedDate = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            // Generate token and return response
            var token = _jwtService.GenerateToken(createdUser);
            var userResponse = _mapper.Map<LoginResponseDto>(createdUser);
            userResponse.Token = token;

            return new RegisterResponseDto
            {
                Success = true,
                Message = "Registration successful",
                User = userResponse
            };
        }

        private string HashPassword(string password)
        {
            // Simple hash for demo - In production, use BCrypt or PBKDF2
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hash = HashPassword(password);
            return hash == passwordHash;
        }
    }
}



















//using AutoMapper;
//using ECommerceAPI.DTOs;
//using ECommerceAPI.Repositories;

//namespace ECommerceAPI.Services
//{
//    public interface IAuthService
//    {
//        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
//    }

//    public class AuthService : IAuthService
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly IJwtService _jwtService;
//        private readonly IMapper _mapper;

//        public AuthService(
//            IUserRepository userRepository,
//            IJwtService jwtService,
//            IMapper mapper)
//        {
//            _userRepository = userRepository;
//            _jwtService = jwtService;
//            _mapper = mapper;
//        }

//        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
//        {
//            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

//            if (user == null)
//                return null;

//            // Simple password verification (In production, use proper hashing like BCrypt)
//            // For demo purposes, password is same as username + "123"
//            var expectedPassword = user.Username + "123";
//            if (loginDto.Password != expectedPassword)
//                return null;

//            var token = _jwtService.GenerateToken(user);
//            var response = _mapper.Map<LoginResponseDto>(user);
//            response.Token = token;

//            return response;
//        }
//    }
//}