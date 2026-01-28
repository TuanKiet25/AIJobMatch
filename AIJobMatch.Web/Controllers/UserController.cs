using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : MyBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Get_All_Users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUserAsync();
            return HandleResult(result);
        }

        [HttpGet("Get_User_By_Id/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return HandleResult(result);
        }

        [HttpPut("Update_User/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            return HandleResult(result);
        }

        [HttpDelete("Delete_User/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return HandleResult(result);
        }
        [HttpPut("Join_Company_By_Code/{inviteCode}")]  
        public async Task<IActionResult> JoinCompanyByCode(string inviteCode)
        {
            var result = await _userService.JoinCompanybyCodeAsync(inviteCode);
            return HandleResult(result);
        }
    }
}
