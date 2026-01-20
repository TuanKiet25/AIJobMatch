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
        Task<int> SaveChangesAsync();
    }
}
