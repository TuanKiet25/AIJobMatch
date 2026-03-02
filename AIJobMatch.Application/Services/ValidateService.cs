using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class ValidateService : IValidateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidateService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> ValidateCandidateSubcription(string subcriptionName)
        {
            try
            {
                string errMessage = "You can not use this feature";
                string successMessage = "Success";
                var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value
                ?? _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return "Can not find User";
                }
                else
                {
                    Guid userId = Guid.Parse(userIdClaim);
                    var candidate = await _unitOfWork.userRepository.GetAsync(
                        filter: c => c.Id == userId,
                        include: c => c.Include(a => a.UserSubscriptions).ThenInclude(p => p.SubscriptionPlans));
                    if(candidate == null)
                    {
                        return "Can not find User";
                    }
                    if (candidate.UserSubscriptions == null || candidate.UserSubscriptions.Count == 0)
                    {
                        return errMessage;
                    }

                    var activeSubscription = candidate.UserSubscriptions
                        .FirstOrDefault(us => us.Status == UserSubscriptionStatus.Active);
                    if (activeSubscription == null)
                    {
                        return "You need subcription to use this feature";
                    }
                    var planName = activeSubscription.SubscriptionPlans?.Name ?? "Unknown Plan";

                    switch(subcriptionName)
                    {
                        case "Plus":
                            if (planName.ToLower().Equals(planName.ToLower()))
                            {
                                return successMessage;
                            }
                            else
                            {
                                return errMessage;   
                            }
                        case "Pro":
                            if(planName.ToLower().Equals(planName.ToLower()))
                            {
                                return successMessage;
                            }
                            else
                            {
                                return errMessage;
                            }
                        default:
                            return errMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error validating candidate subscription: {ex.Message}");
            }
        }

        public Task<bool> ValidateCompanySubcription(string subcriptionName)
        {
            throw new NotImplementedException();
        }
    }
}
