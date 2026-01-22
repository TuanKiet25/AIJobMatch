using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class JobPostingService : IJobPostingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public JobPostingService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<JobPostingResponse> CreateJobPostingAsync(JobPostingRequest request)
        {
            try
            {
                if (request == null)
                    throw new Exception("Job posting request cannot be null");
                
                var company = await _unitOfWork.companyRegister.GetAsync(c => c.Id == request.CompanyId);
                if (company == null)
                    throw new KeyNotFoundException("Company not found");
                
                // Get recruiter ID from JWT token
                var recruiterIdString = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(recruiterIdString) || !Guid.TryParse(recruiterIdString, out var recruiterId))
                    throw new Exception("Invalid recruiter ID from token");

                var recruiter = await _unitOfWork.recruiterRepository.GetAsync(r => r.AccountId == recruiterId);
                if (recruiter == null)
                    throw new KeyNotFoundException("Recruiter not found");

                var jobPosting = _mapper.Map<JobPosting>(request);
                jobPosting.RecruiterId = recruiter.AccountId; 
                
                await _unitOfWork.jobPostingRepository.AddAsync(jobPosting);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<JobPostingResponse>(jobPosting);
                var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                
                response.RecruiterName = _httpContextAccessor.HttpContext.User.FindFirst("FullName")?.Value;
                response.CompanyName = company.Name;
                response.RecruiterId = recruiter.AccountId;
                
                if (address != null)
                {
                    response.Address = new AddressResponse
                    {
                        CityName = address.CityName,
                        DistrictName = address.DistrictName,
                        WardName = address.WardName
                    };
                }
                
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<JobPostingResponse> GetJobPostingByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("Invalid job posting ID");

                var jobPosting = await _unitOfWork.jobPostingRepository.GetAsync(j => j.Id == id);
                if (jobPosting == null)
                    throw new KeyNotFoundException("Job posting not found");

                var response = _mapper.Map<JobPostingResponse>(jobPosting);
                
                // Manually map address from Company
                var company = await _unitOfWork.companyRegister.GetAsync(c => c.Id == jobPosting.CompanyId);
                if (company?.addresses != null && company.addresses.Any())
                {
                    var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                    var Recruiter = await _unitOfWork.recruiterRepository.GetByIdAsync(response.RecruiterId);
                    var user = await _unitOfWork.userRepository.GetByIdAsync(Recruiter.AccountId);
                    response.RecruiterName = user.FullName;
                    response.CompanyName = company.Name;
                    response.Address = new AddressResponse
                    {
                        CityName = address.CityName,
                        DistrictName = address.DistrictName,
                        WardName = address.WardName
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobPostingResponse>> GetAllJobPostingsAsync()
        {
            try
            {
                var jobPostings = await _unitOfWork.jobPostingRepository.GetAllAsync(null);
                if (jobPostings == null || !jobPostings.Any())
                    throw new KeyNotFoundException("No job postings found");

                var responses = _mapper.Map<List<JobPostingResponse>>(jobPostings);
                
                // Manually map addresses for all job postings
                foreach (var response in responses)
                {
                    var jobPosting = jobPostings.FirstOrDefault(j => j.Id == response.Id);
                    if (jobPosting != null)
                    {
                        var company = await _unitOfWork.companyRegister.GetAsync(c => c.Id == jobPosting.CompanyId);
                        if (company?.addresses != null && company.addresses.Any())
                        {
                            var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                            var Recruiter = await _unitOfWork.recruiterRepository.GetByIdAsync(response.RecruiterId);
                            var user = await _unitOfWork.userRepository.GetByIdAsync(Recruiter.AccountId);
                            response.RecruiterName = user.FullName;
                            response.CompanyName = company.Name;
                            response.Address = new AddressResponse
                            {
                                CityName = address.CityName,
                                DistrictName = address.DistrictName,
                                WardName = address.WardName
                            };
                        }
                    }
                }

                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<JobPostingResponse>> GetJobPostingsByCompanyIdAsync(Guid companyId)
        {
            try
            {
                if (companyId == Guid.Empty)
                    throw new Exception("Invalid company ID");

                var jobPostings = await _unitOfWork.jobPostingRepository.GetAllAsync(j => j.CompanyId == companyId);
                if (jobPostings == null || !jobPostings.Any())
                    throw new KeyNotFoundException("No job postings found for this company");

                var responses = _mapper.Map<List<JobPostingResponse>>(jobPostings);
                
                // Manually map address from Company
                var company = await _unitOfWork.companyRegister.GetAsync(c => c.Id == companyId);
                if (company?.addresses != null && company.addresses.Any())
                {
                    var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                    
                    foreach (var response in responses)
                    {
                        var Recruiter = await _unitOfWork.recruiterRepository.GetByIdAsync(response.RecruiterId);
                        var user = await _unitOfWork.userRepository.GetByIdAsync(Recruiter.AccountId);
                        response.RecruiterName = user.FullName;
                        response.CompanyName = company.Name;
                        response.Address = new AddressResponse
                        {
                            CityName = address.CityName,
                            DistrictName = address.DistrictName,
                            WardName = address.WardName
                        };
                    }
                }

                return responses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateJobPostingAsync(Guid id, JobPostingUpdateRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("Invalid job posting ID");

                if (request == null)
                    throw new Exception("Job posting request cannot be null");

                var jobPosting = await _unitOfWork.jobPostingRepository.GetAsync(j => j.Id == id);
                if (jobPosting == null)
                    throw new KeyNotFoundException("Job posting not found");

                _mapper.Map(request, jobPosting);
                await _unitOfWork.jobPostingRepository.UpdateAsync(jobPosting);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteJobPostingAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new Exception("Invalid job posting ID");

                var jobPosting = await _unitOfWork.jobPostingRepository.GetAsync(j => j.Id == id);
                if (jobPosting == null)
                    throw new KeyNotFoundException("Job posting not found");

                await _unitOfWork.jobPostingRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
