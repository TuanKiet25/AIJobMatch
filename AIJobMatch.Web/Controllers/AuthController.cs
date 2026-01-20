using AIJobMatch.Application.IServices;
using AIJobMatch.Application.Services;
using AIJobMatch.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITurnstileService _turnstileService;
        public AuthController(IAuthService authService, ITurnstileService turnstileService)
        {
            _authService = authService;
            _turnstileService = turnstileService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            var isHuman = await _turnstileService.VerifyTokenAsync(request.CaptchaToken);
            if (!isHuman)
            {
                return BadRequest("Xác thực Robot không hợp lệ hoặc đã hết hạn.");
            }
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("company-register")]
        public async Task<IActionResult> CompanyRegister([FromBody] CompanyRegisterRequest request, Guid userId)
        {
            var result = await _authService.CompanyRegisterAsync(request, userId);
            return Ok(result);
        }
    }
}
