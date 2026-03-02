using AIJobMatch.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface IValidateService
    {
        Task<string> ValidateCandidateSubcription(string subcriptionName);
        Task<bool> ValidateCompanySubcription(string subcriptionName);
    }
}
