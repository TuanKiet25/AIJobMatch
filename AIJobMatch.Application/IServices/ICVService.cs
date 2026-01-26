using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface ICVService
    {
        public Task<ServiceResult<CVResponse>> CreateCVAsync(CVRequest cvRequest);
        public Task<ServiceResult<CVResponse>> GetCVByIdAsync(Guid cvId);
        public Task<ServiceResult<CVResponse>> UpdateCVAsync(Guid cvId, CVRequest cvRequest);
        public Task<ServiceResult<string>> DeleteCVAsync(Guid cvId);
        public Task<ServiceResult<List<CVResponse>>> GetAllCVsAsync();
    }
}
