using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IUserService
    {
        public Task<ServiceResult<List<UserResponse>>> GetAllUserAsync();
        public Task<ServiceResult<UserResponse>> GetUserByIdAsync(Guid userId);
        public Task<ServiceResult<string>> UpdateUserAsync(Guid userId, UserUpdateRequest request);
        public Task<ServiceResult<string>> DeleteUserAsync(Guid userId);
    }
}
