using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IAdminDashboardService
    {
        Task<DashboardOverviewResponse> GetDashboardOverviewAsync();
        Task<RevenueSummaryResponse> GetRevenueSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<SubscriptionSalesResponse>> GetSubscriptionSalesAsync();
        Task<List<TopRevenueMonthResponse>> GetTopRevenueMonthsAsync(int monthCount = 12);
        Task<UserStatisticsResponse> GetUserStatisticsAsync();
        Task<List<RecentTransactionResponse>> GetRecentTransactionsAsync(int take = 10);
    }
}
