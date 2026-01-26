using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class CVService : ICVService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CVService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<string>> ActiveCvAsync(Guid cvId, bool isActive)
        {
            try
            {
                var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == cvId && !c.isDeleted);
                if (cv == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "CV not found."
                    };
                }
                cv.IsActive = isActive;
                await _unitOfWork.SaveChangesAsync();
                if (cv.IsActive)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = true,
                        Message = "Active Cv successfully."
                    };
                }
                else
                {
                      return new ServiceResult<string>
                    {
                        IsSuccess = true,
                        Message = "Deactive Cv successfully."
                    };
                }
            }
            catch(Exception ex)
            {
                 return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while updating the CV status: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<CVResponse>> CreateCVAsync(CVRequest cvRequest)
        {
            try
            {
                var candidateIdString = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(candidateIdString) || !Guid.TryParse(candidateIdString, out var candidateId))
                    throw new Exception("Invalid  candidate ID from token");
                var CandidateAccount = await _unitOfWork.userRepository.GetAsync(a => a.Id == candidateId);
                if (CandidateAccount == null)
                {
                    return new ServiceResult<CVResponse>
                    {
                        IsSuccess = false,
                        Message = "Candidate account not found."
                    };
                }
                var cv = _mapper.Map<CandidateProfile>(cvRequest);
                cv.CandidateId = CandidateAccount.Id;
                await _unitOfWork.candidateProfileRepository.AddAsync(cv);
                await _unitOfWork.SaveChangesAsync();   
                var cvResponse = _mapper.Map<CVResponse>(cv);
                cvResponse.CandidateId = CandidateAccount.Id;
                return new ServiceResult<CVResponse>
                {
                    IsSuccess = true,
                    Data = cvResponse,
                    Message = "CV created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CVResponse>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while creating the CV: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<string>> DeleteCVAsync(Guid cvId)
        {
            try
            {
                var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == cvId && !c.isDeleted);
                if (cv == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "CV not found."
                    };
                }        
                await _unitOfWork.candidateProfileRepository.DeleteAsync(cvId);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "CV deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while deleting the CV: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<List<CVResponse>>> GetAllCVsByCandidateIdAsync()
        {
            try
            {
                var candidateIdString = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(candidateIdString) || !Guid.TryParse(candidateIdString, out var candidateId))
                    throw new Exception("Invalid  candidate ID from token");
                var CandidateAccount = await _unitOfWork.userRepository.GetAsync(a => a.Id == candidateId);
                if (CandidateAccount == null)
                {
                    return new ServiceResult<List<CVResponse>>
                    {
                        IsSuccess = false,
                        Message = "Candidate account not found."
                    };
                }
                var cvs = await _unitOfWork.candidateProfileRepository.GetAllAsync(c => c.CandidateId == CandidateAccount.Id && !c.isDeleted, include: 
                    q => q.Include(c => c.Skills)
                          .Include(c => c.WorkExperiences)
                          .Include(c => c.Educations));
                if (cvs == null || cvs.Count == 0)
                {
                    return new ServiceResult<List<CVResponse>>
                    {
                        IsSuccess = false,
                        Message = "No CVs found for the candidate."
                    };
                }
                var cvResponses = _mapper.Map<List<CVResponse>>(cvs);
                
                return new ServiceResult<List<CVResponse>>
                {
                    IsSuccess = true,
                    Data = cvResponses
                };

            }
            catch(Exception ex)
            {
                return new ServiceResult<List<CVResponse>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while retrieving CVs: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult<CVResponse>> GetCVByIdAsync(Guid cvId)
        {
            try
            {
                var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == cvId && !c.isDeleted, include:
                    q => q.Include(c => c.Skills)
                          .Include(c => c.WorkExperiences)
                          .Include(c => c.Educations));
                if (cv == null)
                {
                    return new ServiceResult<CVResponse>
                    {
                        IsSuccess = false,
                        Message = "CV not found."
                    };
                }
                var cvResponse = _mapper.Map<CVResponse>(cv);
                return new ServiceResult<CVResponse>
                {
                    IsSuccess = true,
                    Data = cvResponse
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CVResponse>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while retrieving the CV: {ex.Message}"
                };
            }
        }
        public async Task<ServiceResult<CVResponse>> UpdateCVAsync(Guid cvId, CVRequest cvRequest)
        {
            try
            {
                var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == cvId && !c.isDeleted,include:
                    q => q.Include(c => c.Skills.Where(s => !s.isDeleted))
                          .Include(c => c.WorkExperiences.Where(w => !w.isDeleted))
                          .Include(c => c.Educations.Where(e => !e.isDeleted)));
                if (cv == null)
                {
                    return new ServiceResult<CVResponse>
                    {
                        IsSuccess = false,
                        Message = "CV not found."
                    };
                }

                // Soft delete các item cũ trực tiếp trên entity đã được track
                if (cv.Skills?.Any() == true)
                {
                    foreach (var skill in cv.Skills.ToList())
                    {
                        skill.isDeleted = true;
                        skill.UpdateTime = DateTime.UtcNow;
                    }
                }
                if (cv.WorkExperiences?.Any() == true)
                {
                    foreach (var workEx in cv.WorkExperiences.ToList())
                    {
                        workEx.isDeleted = true;
                        workEx.UpdateTime = DateTime.UtcNow;
                    }
                }
                if (cv.Educations?.Any() == true)
                {
                    foreach (var edu in cv.Educations.ToList())
                    {
                        edu.isDeleted = true;
                        edu.UpdateTime = DateTime.UtcNow;
                    }
                }

                // Map các field từ request (không map collections)
                cv.Jobtitle = cvRequest.Jobtitle;
                cv.AboutMe = cvRequest.AboutMe;
                cv.PortfolioUrl = cvRequest.PortfolioUrl;
                cv.AvatarUrl = cvRequest.AvatarUrl;
                cv.DesiredJobTitle = cvRequest.DesiredJobTitle;
                cv.WorkLocation = cvRequest.WorkLocation;
                cv.JobType = cvRequest.JobType;
                cv.Achievements = cvRequest.Achievements;
                cv.Contacts = cvRequest.Contacts;
                cv.UpdateTime = DateTime.UtcNow;

                // Thêm các item mới vào database
                if (cvRequest.Skills != null && cvRequest.Skills.Any())
                {
                    var newSkills = _mapper.Map<List<Skill>>(cvRequest.Skills);
                    foreach (var skill in newSkills)
                    {
                        skill.ProfileId = cv.Id;
                        skill.Id = Guid.NewGuid();
                        skill.CreateTime = DateTime.UtcNow;
                        skill.UpdateTime = DateTime.UtcNow;
                        skill.isDeleted = false;
                    }
                    await _unitOfWork.skillRepository.AddRangeAsync(newSkills);
                }

                if (cvRequest.WorkExperiences != null && cvRequest.WorkExperiences.Any())
                {
                    var newWorkExperiences = _mapper.Map<List<WorkExperiences>>(cvRequest.WorkExperiences);
                    foreach (var workEx in newWorkExperiences)
                    {
                        workEx.ProfileId = cv.Id;
                        workEx.Id = Guid.NewGuid();
                        workEx.CreateTime = DateTime.UtcNow;
                        workEx.UpdateTime = DateTime.UtcNow;
                        workEx.isDeleted = false;
                    }
                    await _unitOfWork.workExperienceRepository.AddRangeAsync(newWorkExperiences);
                }

                if (cvRequest.Educations != null && cvRequest.Educations.Any())
                {
                    var newEducations = _mapper.Map<List<Education>>(cvRequest.Educations);
                    foreach (var edu in newEducations)
                    {
                        edu.ProfileId = cv.Id;
                        edu.Id = Guid.NewGuid();
                        edu.CreateTime = DateTime.UtcNow;
                        edu.UpdateTime = DateTime.UtcNow;
                        edu.isDeleted = false;
                    }
                    await _unitOfWork.educationRepository.AddRangeAsync(newEducations);
                }

                await _unitOfWork.SaveChangesAsync();

                // Reload CV để lấy data mới nhất cho response
                cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == cvId && !c.isDeleted,
                    include: q => q.Include(c => c.Skills.Where(s => !s.isDeleted))
                                  .Include(c => c.WorkExperiences.Where(w => !w.isDeleted))
                                  .Include(c => c.Educations.Where(e => !e.isDeleted)));

                var cvResponse = _mapper.Map<CVResponse>(cv);
                return new ServiceResult<CVResponse>
                {
                    IsSuccess = true,
                    Data = cvResponse,
                    Message = "CV updated successfully."
                };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return new ServiceResult<CVResponse>
                {
                    IsSuccess = false,
                    Message = $"Concurrency error: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CVResponse>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while updating the CV: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
        }
    }
}
