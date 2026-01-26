using AIJobMatch.Application;
using AIJobMatch.Application.IRepositories;
using AIJobMatch.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly AppDbContext _context;
        public IUserRepository userRepository { get; }
        public ICompanyRegister companyRegister { get; }
        public IRecruiterRepository recruiterRepository { get; }
        public ICityRepository cityRepository { get; }
        public IDistrictRepository districtRepository { get; }
        public IWardRepository wardRepository { get; }
        public IAddressRepository addressRepository { get; }
        public ICandidateRepository candidateRepository { get; }
        public IJobPostingRepository jobPostingRepository { get; }
        public ISubscriptionPlansRepository subscriptionPlansRepository { get; }
        public ISkillRepository skillRepository { get; }
        public ICandidateProfileRepository candidateProfileRepository { get; }
        public IEducationRepository educationRepository { get; }
        public IWorkExperienceRepository workExperienceRepository { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            userRepository = new UserRepository(_context);
            companyRegister = new CompanyRepository(_context);
            recruiterRepository = new RecruiterRepository(_context);
            cityRepository = new CityRepository(_context);
            districtRepository = new DistrictRepository(_context);
            wardRepository = new WardRepository(_context);
            addressRepository = new AddressRepository(_context);
            candidateRepository = new CandidateRepository(_context);
            jobPostingRepository = new JobPostingRepository(_context);
            subscriptionPlansRepository = new SubscriptionPlansRepository(_context);
            skillRepository = new SkillRepository(_context);
            candidateProfileRepository = new CandidateProfileRepository(_context);
            educationRepository = new EducationRepository(_context);
            workExperienceRepository = new WorkExperienceRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
