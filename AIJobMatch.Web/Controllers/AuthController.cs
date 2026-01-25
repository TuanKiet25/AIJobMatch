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

        //XXXX.DUMMY.TOKEN.XXXX
        //XXXX.DUMMY.TOKEN.FAIL.XXXX
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if(!result)
            {
                return BadRequest("Đăng ký không thành công.");
            }
            return Ok("Đăng ký thành công.");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var isHuman = await _turnstileService.VerifyTokenAsync(request.CaptchaToken);
            if (!isHuman)
            {
                return BadRequest("Xác thực Robot không hợp lệ hoặc đã hết hạn.");
            }
            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("company-register")]
        public async Task<IActionResult> CompanyRegister([FromBody] CompanyRegisterRequest request)
        {
            var result = await _authService.CompanyRegisterAsync(request);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("create-invite-code")]
        public async Task<IActionResult> CreateInviteCode([FromBody] string inviteCode)
        {
            try
            {
                var result = await _authService.CreateCompanyInviteCodeAsync(inviteCode);
                if (!result)
                {
                    return BadRequest("Mã mời không được để trống");
                }
                return Ok("Tạo mã mời thành công.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Công Ty hoặc Nhà tuyển dụng không tồn tại!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
