using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Filter
{
    public class PremiumDateCheckAttribute : TypeFilterAttribute
    {
        public PremiumDateCheckAttribute() : base(typeof(PremiumCheckFilter)) { }
        
    }

    public class PremiumCheckFilter : IAuthorizationFilter
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PremiumCheckFilter(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value
                ?? _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasActiveSubscription = _context.UserSubscriptions
                .Any(us => us.UserId.ToString() == userIdClaim 
                && us.Status == Domain.Enums.UserSubscriptionStatus.Active 
                && us.ExpirationDate > DateTime.UtcNow);

            if (!hasActiveSubscription)
            {
                context.Result = new ObjectResult("Gói Premium đã hết hạn hoặc không tồn tại")
                {
                    StatusCode = 403
                };
            }
        }
    }
}
