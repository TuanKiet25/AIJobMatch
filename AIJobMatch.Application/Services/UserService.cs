using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<List<UserResponse>>> GetAllUserAsync()
        {
            try
            {
                var users = await _unitOfWork.userRepository.GetAllAsync(u => !u.isDeleted);
                if (users == null || users.Count == 0)
                {
                    return new ServiceResult<List<UserResponse>>
                    {
                        Data = null,
                        IsNotFound = true,
                        IsSuccess = false,
                        Message = "No users found!!"
                    };
                }
                var responses = _mapper.Map<List<UserResponse>>(users);
                return new ServiceResult<List<UserResponse>>
                {
                    Data = responses,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<UserResponse>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<UserResponse>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<UserResponse>
                    {
                        Data = null,
                        IsNotFound = true,
                        IsSuccess = false,
                        Message = "User not found!!"
                    };
                }
                var response = _mapper.Map<UserResponse>(user);
                return new ServiceResult<UserResponse>
                {
                    Data = response,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserResponse>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<string>> UpdateUserAsync(Guid userId, UserUpdateRequest request)
        {
            try
            {
                var user = await _unitOfWork.userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        IsNotFound = true
                    };
                }

                _mapper.Map(request, user);
                user.UpdateTime = DateTime.UtcNow;

                await _unitOfWork.userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "User updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<string>> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "User not found",
                        IsNotFound = true
                    };
                }

                await _unitOfWork.userRepository.DeleteAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
