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
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminDashboardService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DashboardOverviewResponse> GetDashboardOverviewAsync()
        {
            var transactions = await _unitOfWork.transactionRepository.GetAllAsync(
                filter: t => t.TransactionStatus == TransactionStatus.Completed);

            var userSubscriptions = await _unitOfWork.userSubsriptionRepository.GetAllAsync(
                filter: us => us.Status == UserSubscriptionStatus.Active);

            var thisMonth = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(thisMonth.Year, thisMonth.Month, 1);
            var lastMonth = firstDayOfMonth.AddMonths(-1);
            var firstDayOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);

            var thisMonthTransactions = transactions
                .Where(t => t.CreateTime >= firstDayOfMonth && t.CreateTime < firstDayOfNextMonth)
                .ToList();

            var lastMonthTransactions = transactions
                .Where(t => t.CreateTime >= firstDayOfLastMonth && t.CreateTime < firstDayOfMonth)
                .ToList();

            decimal thisMonthRevenue = thisMonthTransactions.Sum(t => t.Amount);
            decimal lastMonthRevenue = lastMonthTransactions.Sum(t => t.Amount);
            decimal growthPercentage = lastMonthRevenue > 0
                ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100
                : 0;

            // Get new users this month
            var users = await _unitOfWork.userRepository.GetAllAsync(
                filter: null);
            var newUsersThisMonth = users
                .Where(u => u.CreateTime >= firstDayOfMonth && u.CreateTime < firstDayOfNextMonth)
                .Count();

            return new DashboardOverviewResponse
            {
                TotalRevenue = transactions.Sum(t => t.Amount),
                TotalTransactions = transactions.Count(),
                TotalSubscribedUsers = userSubscriptions.Select(us => us.UserId).Distinct().Count(),
                TotalSubscriptionsSold = userSubscriptions.Count(),
                ThisMonthRevenue = thisMonthRevenue,
                ThisMonthTransactions = thisMonthTransactions.Count(),
                LastMonthRevenue = lastMonthRevenue,
                GrowthPercentage = growthPercentage,
                NewUsersThisMonth = newUsersThisMonth
            };
        }

        public async Task<RevenueSummaryResponse> GetRevenueSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.UtcNow.AddMonths(-1);
            endDate = endDate ?? DateTime.UtcNow;

            var transactions = await _unitOfWork.transactionRepository.GetAllAsync(
                filter: t => t.TransactionStatus == TransactionStatus.Completed
                    && t.CreateTime >= startDate && t.CreateTime <= endDate);

            decimal totalRevenue = transactions.Sum(t => t.Amount);
            int completedCount = transactions.Count();

            return new RevenueSummaryResponse
            {
                TotalRevenue = totalRevenue,
                CompletedTransactions = completedCount,
                AverageTransactionAmount = completedCount > 0 ? totalRevenue / completedCount : 0,
                PeriodStart = startDate.Value,
                PeriodEnd = endDate.Value
            };
        }

        public async Task<List<SubscriptionSalesResponse>> GetSubscriptionSalesAsync()
        {
            var plans = await _unitOfWork.subscriptionPlansRepository.GetAllAsync(
                filter: null);
            var transactions = await _unitOfWork.transactionRepository.GetAllAsync(
                filter: t => t.TransactionStatus == TransactionStatus.Completed);
            var userSubscriptions = await _unitOfWork.userSubsriptionRepository.GetAllAsync(
                filter: null);

            decimal totalRevenue = transactions.Sum(t => t.Amount);

            var salesList = new List<SubscriptionSalesResponse>();

            foreach (var plan in plans)
            {
                var planTransactions = transactions.Where(t => t.PlanId == plan.Id).ToList();
                var planRevenue = planTransactions.Sum(t => t.Amount);
                var totalSold = planTransactions.Count();
                var activeCount = userSubscriptions.Count(us => us.PlanId == plan.Id && us.Status == UserSubscriptionStatus.Active);
                var expiredCount = userSubscriptions.Count(us => us.PlanId == plan.Id && us.Status != UserSubscriptionStatus.Active);

                salesList.Add(new SubscriptionSalesResponse
                {
                    PlanId = plan.Id,
                    PlanName = plan.Name,
                    PlanPrice = plan.Price,
                    TotalSold = totalSold,
                    TotalRevenue = planRevenue,
                    PercentageOfTotal = totalRevenue > 0 ? (planRevenue / totalRevenue) * 100 : 0,
                    ActiveSubscriptions = activeCount,
                    ExpiredSubscriptions = expiredCount
                });
            }

            return salesList.OrderByDescending(x => x.TotalRevenue).ToList();
        }

        public async Task<List<TopRevenueMonthResponse>> GetTopRevenueMonthsAsync(int monthCount = 12)
        {
            var transactions = await _unitOfWork.transactionRepository.GetAllAsync(
                filter: t => t.TransactionStatus == TransactionStatus.Completed);

            var monthlyRevenue = transactions
                .GroupBy(t => new { Year = t.CreateTime.Year, Month = t.CreateTime.Month })
                .Select(g => new TopRevenueMonthResponse
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Revenue = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(x => x.Revenue)
                .Take(monthCount)
                .OrderBy(x => x.Month)
                .ToList();

            return monthlyRevenue;
        }

        public async Task<UserStatisticsResponse> GetUserStatisticsAsync()
        {
            var users = await _unitOfWork.userRepository.GetAllAsync(
                filter: null);
            var userSubscriptions = await _unitOfWork.userSubsriptionRepository.GetAllAsync(
                filter: null);

            int totalUsers = users.Count();
            int candidateUsers = users.Count(u => u.Role == Role.Candidate);
            int recruiterUsers = users.Count(u => u.Role == Role.recruiter);
            int usersWithActiveSubscription = userSubscriptions
                .Where(us => us.Status == UserSubscriptionStatus.Active)
                .Select(us => us.UserId)
                .Distinct()
                .Count();
            int usersWithExpiredSubscription = userSubscriptions
                .Where(us => us.Status != UserSubscriptionStatus.Active)
                .Select(us => us.UserId)
                .Distinct()
                .Count();

            decimal conversionRate = totalUsers > 0 
                ? ((usersWithActiveSubscription + usersWithExpiredSubscription) / (decimal)totalUsers) * 100 
                : 0;

            return new UserStatisticsResponse
            {
                TotalUsers = totalUsers,
                CandidateUsers = candidateUsers,
                RecruiterUsers = recruiterUsers,
                UsersWithActiveSubscription = usersWithActiveSubscription,
                UsersWithExpiredSubscription = usersWithExpiredSubscription,
                SubscriptionConversionRate = conversionRate
            };
        }

        public async Task<List<RecentTransactionResponse>> GetRecentTransactionsAsync(int take = 10)
        {
            var transactions = await _unitOfWork.transactionRepository.GetAllAsync(
                filter: null);

            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value
                ?? _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                userIdClaim = "UnknownUser";
            }

            var recentTransactions = transactions
                .OrderByDescending(t => t.CreateTime)
                .Take(take)
                .Select(t => new RecentTransactionResponse
                {
                    TransactionId = t.Id,
                    UserId = userIdClaim,
                    PlanId = t.PlanId,
                    Amount = t.Amount,
                    Status = t.TransactionStatus.ToString(),
                    CreatedDate = t.CreateTime
                })
                .ToList();

            return recentTransactions;
        }
    }
}
