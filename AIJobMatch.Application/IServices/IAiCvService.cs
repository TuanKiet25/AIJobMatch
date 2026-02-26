using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IAiCvService
    {
        public Task<ServiceResult<AiCvReviewResponse>> SuggestCvDataAsync(CVRequest cVRequest);
        public Task<ServiceResult<AiCvReviewResponse>> SuggestCvDataByIdAsync(Guid id);
    }
}
