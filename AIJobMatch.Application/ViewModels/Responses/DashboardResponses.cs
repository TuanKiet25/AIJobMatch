using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class DashboardOverviewResponse
    {
        /// <summary>T?ng doanh thu</summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>T?ng s? giao d?ch thành công</summary>
        public int TotalTransactions { get; set; }

        /// <summary>T?ng s? user ?ã mua subscription</summary>
        public int TotalSubscribedUsers { get; set; }

        /// <summary>S? gói subscription ?ã bán</summary>
        public int TotalSubscriptionsSold { get; set; }

        /// <summary>Doanh thu tháng này</summary>
        public decimal ThisMonthRevenue { get; set; }

        /// <summary>S? giao d?ch tháng này</summary>
        public int ThisMonthTransactions { get; set; }

        /// <summary>Doanh thu tháng tr??c</summary>
        public decimal LastMonthRevenue { get; set; }

        /// <summary>T? l? t?ng tr??ng (%) so v?i tháng tr??c</summary>
        public decimal GrowthPercentage { get; set; }

        /// <summary>S? user m?i tháng này</summary>
        public int NewUsersThisMonth { get; set; }
    }

    public class RevenueSummaryResponse
    {
        public decimal TotalRevenue { get; set; }
        public int CompletedTransactions { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class SubscriptionSalesResponse
    {
        public Guid PlanId { get; set; }
        public string PlanName { get; set; }
        public decimal PlanPrice { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int ExpiredSubscriptions { get; set; }
    }

    public class TopRevenueMonthResponse
    {
        public DateTime Month { get; set; }
        public decimal Revenue { get; set; }
        public int TransactionCount { get; set; }
    }

    public class UserStatisticsResponse
    {
        public int TotalUsers { get; set; }
        public int CandidateUsers { get; set; }
        public int RecruiterUsers { get; set; }
        public int UsersWithActiveSubscription { get; set; }
        public int UsersWithExpiredSubscription { get; set; }
        public decimal SubscriptionConversionRate { get; set; } // %
    }

    public class RecentTransactionResponse
    {
        public Guid TransactionId { get; set; }
        public string UserEmail { get; set; }
        public string PlanName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
