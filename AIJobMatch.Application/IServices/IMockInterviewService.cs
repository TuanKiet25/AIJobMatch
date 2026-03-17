using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IMockInterviewService
    {
         Task<ServiceResult<StartInterviewResponse>> StartMockInterviewAsync(StartInterviewRequest request);
         Task<ServiceResult<MockInterviewResponse>> ChatInterviewAsync(MockInterviewRequest request);
         Task<ServiceResult<MockInterviewResultResponse>> EvaluateInterviewAsync(Guid MockInterviewId);
         Task<ServiceResult<MockInterviewResultResponse>> GetMockInterviewResultAsync(Guid mockInterviewId);
    }
}
