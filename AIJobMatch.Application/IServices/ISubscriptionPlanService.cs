using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface ISubscriptionPlanService
    {
        Task<SubscriptionPlanResponse> CreateAsync(SubscriptionPlanRequest request);
        Task<SubscriptionPlanResponse> GetByIdAsync(Guid id);
        Task<List<SubscriptionPlanResponse>> GetAllAsync();
        Task<List<SubscriptionPlanResponse>> GetAllEvenDeletedAsync();
        Task<SubscriptionPlanResponse> UpdateAsync(Guid id, SubscriptionPlanRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
