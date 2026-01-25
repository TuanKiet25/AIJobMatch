using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AIJobMatch.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<string>> DeleteCompanyAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.companyRegister.GetByIdAsync(companyId);
                if (company == null) return new ServiceResult<string> { IsSuccess = false, Message = "Company not found", IsNotFound = true };
                await _unitOfWork.companyRegister.DeleteAsync(companyId);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string> { IsSuccess = true, Message = "Company deleted successfully" };

            }
            catch (Exception ex)
            {
                return new ServiceResult<string> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<List<CompanyRegisterResponse>>> GetAllCompanyAsync()
        {
            try
            { 
                var companies = await _unitOfWork.companyRegister.GetAllAsync( c => !c.isDeleted);
                if (companies == null || companies.Count == 0)
                {
                    return new ServiceResult<List<CompanyRegisterResponse>> { Data = null, IsNotFound = true, IsSuccess = false, Message = " No company existed !!" };
                }
                var responses = _mapper.Map<List<CompanyRegisterResponse>>(companies);
                foreach (var response in responses)
                {
                    var company = await _unitOfWork.companyRegister.GetByIdAsync(response.Id);
                    var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                    response.Address = new AddressResponse()
                    {     
                        CityName = address.CityName,
                        DistrictName = address.DistrictName,
                        WardName = address.WardName
                    };
                }
                return new ServiceResult<List<CompanyRegisterResponse>> { Data = responses, IsSuccess = true};
            }
            catch (Exception ex)
            {
               return new ServiceResult<List<CompanyRegisterResponse>> { Data = null, IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<List<CompanyRegisterResponse>>> GetAllCompanyPedingVerifyAsync()
        {
            try
            {
                var result = await _unitOfWork.companyRegister.GetAllAsync(c => c.VerificationStatus == VerificationStatus.Pending && !c.isDeleted);    
                if (result == null || result.Count == 0)
                {
                    return new ServiceResult<List<CompanyRegisterResponse>> { Data = null, IsNotFound = true, IsSuccess = false, Message = " No company pending verify !!" };
                }
                var responses = _mapper.Map<List<CompanyRegisterResponse>>(result);
                foreach (var response in responses)
                {
                    var company = await _unitOfWork.companyRegister.GetByIdAsync(response.Id);
                    var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                    response.Address = new AddressResponse()
                    {
                        CityName = address.CityName,
                        DistrictName = address.DistrictName,
                        WardName = address.WardName
                    };
                }
                return new ServiceResult<List<CompanyRegisterResponse>> { Data = responses, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<CompanyRegisterResponse>> { Data = null, IsSuccess = false, Message = ex.Message };
            }   

        }

        public async Task< ServiceResult<CompanyRegisterResponse>> GetCompanyByIdAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.companyRegister.GetByIdAsync(companyId);
                if (company == null)
                {
                    return new ServiceResult<CompanyRegisterResponse> { Data = null, IsNotFound = true, IsSuccess = false, Message = " Company not found !!" };
                }
                var response = _mapper.Map<CompanyRegisterResponse>(company);
                var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                response.Address = new AddressResponse()
                {
                    CityName = address.CityName,
                    DistrictName = address.DistrictName,
                    WardName = address.WardName
                };
                return new ServiceResult<CompanyRegisterResponse> { Data = response, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CompanyRegisterResponse> { Data = null, IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<CompanyRegisterResponse>> GetCompanyByRecruiterId(Guid id)
        {
            try
            {
                var user = await _unitOfWork.userRepository.GetByIdAsync(id);
                var recruiter = await _unitOfWork.recruiterRepository.GetAsync(r => r.AccountId == user.Id);
                if (recruiter == null) return new ServiceResult<CompanyRegisterResponse> { Data = null, IsNotFound = true, IsSuccess = false, Message = "Recruiter not found" };
                var company = await _unitOfWork.companyRegister.GetAsync(c => c.Id == recruiter.CompanyId);
                if (company == null)
                {
                    return new ServiceResult<CompanyRegisterResponse> { Data = null, IsNotFound = true, IsSuccess = false, Message = " Company not found !!" };
                }
                var response = _mapper.Map<CompanyRegisterResponse>(company);
                var address = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == company.Id);
                response.Address = new AddressResponse()
                {
                    CityName = address.CityName,
                    DistrictName = address.DistrictName,
                    WardName = address.WardName
                };
                return new ServiceResult<CompanyRegisterResponse> { Data = response, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CompanyRegisterResponse> { Data = null, IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<string>> GetCompanyInviteCodeAsync(Guid companyId)
        {
            try
            {
                var company = await _unitOfWork.companyRegister.GetByIdAsync(companyId);    
                if (company == null) return new ServiceResult<string> { IsSuccess = false, Message = "Company not found", IsNotFound = true };
                return new ServiceResult<string> { IsSuccess = true, Data = $"Your company invite code: {company.InviteCode}" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string> { IsSuccess = false, Message = ex.Message };
            }   
        }

        public async Task<ServiceResult<string>> UpdateCompanyAsync(Guid companyId, CompanyUpdateResquest request)
        {
            try
            {
                var company = await _unitOfWork.companyRegister.GetByIdAsync(companyId);
                if (company == null) return new ServiceResult<string> { IsSuccess = false, Message = "Company not found", IsNotFound = true };
                //handle address
                var city = await _unitOfWork.cityRepository.GetAsync(c => c.CityCode == request.Address.CityCode);
                var district = await _unitOfWork.districtRepository.GetAsync(c => c.DistrictCode == request.Address.DistrictCode);
                var ward = await _unitOfWork.wardRepository.GetAsync(c => c.WardCode == request.Address.WardCode);
                var existingAddress = await _unitOfWork.addressRepository.GetAsync(a => a.CompanyId == companyId);

                existingAddress.Street = request.Address.Street;
                existingAddress.CityCode = city.CityCode;
                existingAddress.CityName = city.CityName;
                existingAddress.DistrictCode = district.DistrictCode;
                existingAddress.DistrictName = district.DistrictName;
                existingAddress.WardCode = ward.WardCode;
                existingAddress.WardName = ward.WardName;

                _mapper.Map(request, company);

                await _unitOfWork.companyRegister.UpdateAsync(company); 
                await _unitOfWork.addressRepository.UpdateAsync(existingAddress);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string> { IsSuccess = true, Message = "Company updated successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string> {  IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<string>> VerifyCompanyAsync(Guid companyId, VerificationStatus verificationStatus, string mess)
        {
            try
            {
                var company = await _unitOfWork.companyRegister.GetByIdAsync(companyId);
                if (company == null) return new ServiceResult<string> { IsSuccess = false, Message = "Company not found", IsNotFound = true };
                company.VerificationStatus = verificationStatus;
                company.RejectionReason = mess;
                company.VerifiedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string> { IsSuccess = true, Message = "Company verification successfully" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string> { IsSuccess = false, Message = ex.Message };
            }
        }

        
    }  
}
