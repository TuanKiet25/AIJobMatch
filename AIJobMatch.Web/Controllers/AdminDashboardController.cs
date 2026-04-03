using AIJobMatch.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Lấy tổng quan dashboard (tổng doanh thu, số giao dịch, v.v.)
        /// </summary>
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            try
            {
                var overview = await _dashboardService.GetDashboardOverviewAsync();
                return Ok(new { success = true, data = overview });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tóm tắt doanh thu trong khoảng thời gian
        /// </summary>
        [HttpGet("revenue-summary")]
        public async Task<IActionResult> GetRevenueSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var summary = await _dashboardService.GetRevenueSummaryAsync(startDate, endDate);
                return Ok(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê bán hàng theo từng gói subscription
        /// </summary>
        [HttpGet("subscription-sales")]
        public async Task<IActionResult> GetSubscriptionSales()
        {
            try
            {
                var sales = await _dashboardService.GetSubscriptionSalesAsync();
                return Ok(new { success = true, data = sales });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy doanh thu top months
        /// </summary>
        [HttpGet("top-revenue-months")]
        public async Task<IActionResult> GetTopRevenueMonths([FromQuery] int monthCount = 12)
        {
            try
            {
                var topMonths = await _dashboardService.GetTopRevenueMonthsAsync(monthCount);
                return Ok(new { success = true, data = topMonths });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thống kê user
        /// </summary>
        [HttpGet("user-statistics")]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var stats = await _dashboardService.GetUserStatisticsAsync();
                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách giao dịch gần đây
        /// </summary>
        [HttpGet("recent-transactions")]
        public async Task<IActionResult> GetRecentTransactions([FromQuery] int take = 10)
        {
            try
            {
                var transactions = await _dashboardService.GetRecentTransactionsAsync(take);
                return Ok(new { success = true, data = transactions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
