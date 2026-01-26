using AIJobMatch.Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application
{
    public interface IUnitOfWork
    {
        IUserRepository userRepository { get; }
        ICompanyRegister companyRegister { get; }
        IRecruiterRepository recruiterRepository { get; }
        ICityRepository cityRepository { get; }
        IDistrictRepository districtRepository { get; }
        IWardRepository wardRepository { get; }
        IAddressRepository addressRepository { get; }
        ICandidateRepository candidateRepository { get; }
        IJobPostingRepository jobPostingRepository { get; }
        ISubscriptionPlansRepository subscriptionPlansRepository { get; }
        ITransactionRepository transactionRepository { get; }
        IUserSubsriptionRepository userSubsriptionRepository { get; }
        ISkillRepository skillRepository { get; }
        ICandidateProfileRepository candidateProfileRepository { get; }
        IEducationRepository educationRepository { get; }
        IWorkExperienceRepository workExperienceRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
