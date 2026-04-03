using AIJobMatch.Application;
using AIJobMatch.Application.IRepositories;
using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AIJobMatch.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class SubscriptionPlanService : ISubscriptionPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubscriptionPlanService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SubscriptionPlanResponse> CreateAsync(SubscriptionPlanRequest request)
        {
            try
            {
                if (request == null) throw new Exception("Null request");

                // Check if plan with same name already exists
                var existingPlan = await _unitOfWork.subscriptionPlansRepository.GetAsync(sp => sp.Name == request.Name && !sp.isDeleted);
                if (existingPlan != null)
                {
                    throw new Exception("Subscription plan with this name already exists.");
                }

                var entity = _mapper.Map<SubscriptionPlans>(request);
                await _unitOfWork.subscriptionPlansRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<SubscriptionPlanResponse>(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating subscription plan: {ex.Message}");
            }
        }

        public async Task<SubscriptionPlanResponse> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.subscriptionPlansRepository.GetByIdAsync(id);
                if (entity == null || entity.isDeleted)
                {
                    throw new Exception("Subscription plan not found.");
                }
                return _mapper.Map<SubscriptionPlanResponse>(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subscription plan: {ex.Message}");
            }
        }

        public async Task<List<SubscriptionPlanResponse>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.subscriptionPlansRepository.GetAllAsync(sp => !sp.isDeleted);
                return _mapper.Map<List<SubscriptionPlanResponse>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subscription plans: {ex.Message}");
            }
        }

        public async Task<SubscriptionPlanResponse> UpdateAsync(Guid id, SubscriptionPlanRequest request)
        {
            try
            {
                if (request == null) throw new Exception("Null request");

                var entity = await _unitOfWork.subscriptionPlansRepository.GetByIdAsync(id);
                if (entity == null || entity.isDeleted)
                {
                    throw new Exception("Subscription plan not found.");
                }
                else if (entity != null && !entity.isDeleted && entity.Status == SubscriptionPlansStatus.Active)
                {
                    throw new Exception("Active subscription plans cannot be updated.");
                }

                // Check if another plan with same name exists (excluding current one)
                //sửa lại phần này vì khi không cho sửa tên luôn
                var existingPlan = await _unitOfWork.subscriptionPlansRepository.GetAsync(sp => sp.Name == request.Name && sp.Id != id && !sp.isDeleted);
                if (existingPlan != null)
                {
                    throw new Exception("Subscription plan with this name already exists.");
                }

                entity.Name = request.Name;
                entity.TargetRole = request.TargetRole;
                entity.Price = request.Price;
                entity.DurationInDays = request.DurationInDays;
                entity.Features = request.Features;
                entity.Status = request.Status;
                entity.UpdateTime = DateTime.UtcNow;

                await _unitOfWork.subscriptionPlansRepository.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<SubscriptionPlanResponse>(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating subscription plan: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.subscriptionPlansRepository.GetByIdAsync(id);
                if (entity == null || entity.isDeleted)
                {
                    throw new Exception("Subscription plan not found.");
                }

                await _unitOfWork.subscriptionPlansRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting subscription plan: {ex.Message}");
            }
        }

        public async Task<List<SubscriptionPlanResponse>> GetAllEvenDeletedAsync()
        {
            try
            {
                var entities = await _unitOfWork.subscriptionPlansRepository.GetAllAsync(a => a.Id != null);
                return _mapper.Map<List<SubscriptionPlanResponse>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subscription plans: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> ChangeStatusToInactiveAsync(Guid id)
        {
            try
            {
                var subPlan = await _unitOfWork.subscriptionPlansRepository.GetByIdAsync(id);
                if (subPlan == null || subPlan.isDeleted)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Subscription plan not found."
                    };
                }
                subPlan.Status = SubscriptionPlansStatus.Inactive;
                subPlan.UpdateTime = DateTime.UtcNow;
                await _unitOfWork.subscriptionPlansRepository.UpdateAsync(subPlan);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Subscription plan status changed to Inactive successfully."
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error changing subscription plan status: {ex.Message}");
            }
        }
        public async Task<ServiceResult<List<UserSubscriptionResponse>>> GetAllUserWithSubplanAsync()
        {
            try
            {
                var userSubscriptions = await _unitOfWork.userRepository.GetAllAsync(a => a.UserSubscriptions.Any(us => !us.isDeleted), include: u => u.Include(us => us.UserSubscriptions).ThenInclude(s => s.SubscriptionPlans));
                var responses = _mapper.Map<List<UserSubscriptionResponse>>(userSubscriptions);
                return new ServiceResult<List<UserSubscriptionResponse>>
                {
                    Data = responses,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<UserSubscriptionResponse>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ServiceResult<List<UserSubscriptionResponse>>> GetSubcriptionPlanByUserAsync()
        {
            try
            {
                var UserIdString = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(UserIdString) || !Guid.TryParse(UserIdString, out var userId))
                {
                    return new ServiceResult<List<UserSubscriptionResponse>>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "Invalid  user ID from token"
                    };
                }

                var userSubscriptions = await _unitOfWork.userSubsriptionRepository.GetAllAsync(us => !us.isDeleted && us.UserId == userId, include: u => u.Include(us => us.SubscriptionPlans));
                var responses = _mapper.Map<List<UserSubscriptionResponse>>(userSubscriptions);
                return new ServiceResult<List<UserSubscriptionResponse>>
                {
                    Data = responses,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<UserSubscriptionResponse>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
