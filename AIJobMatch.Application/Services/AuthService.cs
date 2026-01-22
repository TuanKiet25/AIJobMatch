using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AIJobMatch.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<CompanyRegisterResponse> CompanyRegisterAsync(CompanyRegisterRequest request, Guid userId)
        {
            try
            {
                if(request == null) throw new Exception("Null request");
                if(await _unitOfWork.companyRegister.GetAsync(n => n.TaxCode == request.TaxCode) != null)
                {
                    throw new Exception("Tax Code already exists.");
                }

                var companyEntity = _mapper.Map<Company>(request);
                var companyResponse = _mapper.Map<CompanyRegisterResponse>(companyEntity);

                var recruiter = await _unitOfWork.recruiterRepository.GetAsync(r => r.AccountId == userId);
                if(recruiter == null)
                {
                    throw new Exception("Not found Account.");
                }
                recruiter.CompanyId = companyResponse.Id;

                await _unitOfWork.recruiterRepository.UpdateAsync(recruiter);
                await _unitOfWork.companyRegister.AddAsync(companyEntity);
                await _unitOfWork.SaveChangesAsync();

                return companyResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<string> LoginAsync(LoginRequest request)
        {
            try
            {
                if(request == null) throw new Exception("Null request");
                if(string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.PasswordHash))
                {
                    throw new Exception("Email and Password cannot be empty.");
                }

                var account = await _unitOfWork.userRepository.GetAsync(u => u.Email == request.Email && !u.isDeleted);
                if(account == null || !BCrypt.Net.BCrypt.Verify(request.PasswordHash, account.PasswordHash))
                {
                    throw new Exception("Incorrect Email or Password.");
                }

                // 3. TẠO TOKEN (Generate Token)
                var jwtTokenHandler = new JwtSecurityTokenHandler();

                // Lấy Secret Key từ Config (Đảm bảo giống hệt trong Program.cs)
                var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]);

                var tokenDescription = new SecurityTokenDescriptor
                {
                    // A. Payload (Chứa thông tin user)
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", account.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                        new Claim(ClaimTypes.Role, account.Role.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                        // Nếu có Role thì thêm vào đây:
                        // new Claim(ClaimTypes.Role, account.Role) 
                    }),

                    // B. Thời gian hết hạn (Lấy từ config hoặc fix cứng)
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtConfig:AccessTokenExpiration"])),

                    // C. Thuật toán ký (Quan trọng)
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature),

                    // D. Issuer & Audience (Phải khớp với cấu hình ở Program.cs nếu có check)
                    Issuer = _configuration["JwtConfig:Issuer"],
                    Audience = _configuration["JwtConfig:Audience"]
                };

                // 4. Tạo token và chuyển thành chuỗi string
                var tokenObj = jwtTokenHandler.CreateToken(tokenDescription);
                var jwtToken = jwtTokenHandler.WriteToken(tokenObj);

                return jwtToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (request == null) throw new Exception("Null request");
                var existingAccount = await _unitOfWork.userRepository.GetAsync(u => (u.Email == request.Email || u.PhoneNumber == request.PhoneNumber) && !u.isDeleted);
                if (existingAccount != null)
                {
                    return "Account with given email or phone number already exists.";
                }

                var account = _mapper.Map<Account>(request);
                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);
                
                var recruiter = new Recruiter
                {
                    AccountId = account.Id
                };
                await _unitOfWork.recruiterRepository.AddAsync(recruiter);

                await _unitOfWork.userRepository.AddAsync(account);
                await _unitOfWork.SaveChangesAsync();

                return "Registration successful.";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
