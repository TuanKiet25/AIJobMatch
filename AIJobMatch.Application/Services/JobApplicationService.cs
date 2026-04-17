using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AIJobMatch.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor; 
        private readonly string _apiKey;
        private readonly string _model;
        public JobApplicationService(IUnitOfWork unitOfWork, IConfiguration config, IMapper mapper, IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _apiKey = config["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("Thiếu cấu hình Gemini API Key");
            _model = config["GeminiAI:Model"] ?? "gemini-2.5-flash";
        }

        public class AIMatchResult
        {
            public double MatchScore { get; set; }
            public string AIAnalysis { get; set; }
        }

        private async Task<AIMatchResult> CallGeminiAiAsync(string cvSnapshot, string jdJson)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            var prompt = $@"
        Bạn là một hệ thống ATS chuyên nghiệp. Đánh giá mức độ phù hợp giữa CV và JD.
        [DỮ LIỆU JD]: {jdJson}
        [DỮ LIỆU CV]: {cvSnapshot}

        TIÊU CHÍ: 1. Kỹ năng (React, .NET,...), 2. Kinh nghiệm (Years), 3. Học vấn.
        YÊU CẦU ĐẦU RA: Trả về JSON thuần túY (KHÔNG markdown):
        {{
            ""MatchScore"": 85.5,
            ""AIAnalysis"": ""{{ \""Strengths\"": [\""...\""], \""Weaknesses\"": [\""...\""], \""Summary\"": \""...\"" }}""
        }}";

            var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            // Gọi API
            var response = await _httpClient.PostAsync(url, jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google API Error: {response.StatusCode} - {responseString}");
            }

            // Bóc tách JSON theo cấu trúc của Google Gemini
            using var googleDoc = JsonDocument.Parse(responseString);
            if (!googleDoc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                throw new Exception("AI không trả về kết quả (có thể do Safety Filter).");
            }

            string aiRawText = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            // Dọn dẹp Markdown nếu có
            aiRawText = aiRawText.Replace("```json", "").Replace("```", "").Trim();

            // Parse kết quả thực sự từ AI
            using var aiDoc = JsonDocument.Parse(aiRawText);
            var root = aiDoc.RootElement;

            return new AIMatchResult
            {
                MatchScore = root.TryGetProperty("MatchScore", out var s) ? s.GetDouble() :
                             (root.TryGetProperty("matchScore", out var ls) ? ls.GetDouble() : 0),

                AIAnalysis = root.TryGetProperty("AIAnalysis", out var a) ?
                             (a.ValueKind == JsonValueKind.String ? a.GetString() : a.GetRawText()) : ""
            };
        }


        public async Task<ServiceResult<bool>> AnalyzeAndSaveAiResultAsync(Guid jobApplicationId)
        {
            try
            {
                // 1. Lấy JobApplication cùng với dữ liệu cần thiết từ Snapshot
                var application = await _unitOfWork.jobApplicationRepository.GetAsync(
                    a => a.Id == jobApplicationId,
                    q => q.Include(a => a.JobPosting)
                    // Để lấy thông tin JD gốc nếu cần
                );

                if (application == null)
                    return new ServiceResult<bool> { IsSuccess = false, Message = "Application not found." };

                // 2. Trích xuất thông tin từ Snapshot (Dữ liệu đã lưu lúc apply)
                // Lưu ý: Dùng chính Snapshot giúp đảm bảo tính công bằng 
                // (AI đánh giá đúng những gì ứng viên nộp lúc đó, dù sau này họ có sửa CV)
                var cvSnapshot = application.ProfilesSnapshot;

                // Lấy thông tin JD (Có thể lấy từ DB hoặc từ một Object truyền vào)
                var jdData = await _unitOfWork.jobPostingRepository.GetAsync(j => j.Id == application.JobPostingId && !j.isDeleted);
                var jdResponse = _mapper.Map<JobPostingResponse>(jdData);
                var jJdData = JsonSerializer.Serialize(jdResponse);
                // 3. Thực hiện gọi AI (Sử dụng logic gọi Google Gemini của bạn)
                // --- Đoạn này bạn copy logic gọi HttpClient và Parse JSON từ code cũ ---
                var aiResult = await CallGeminiAiAsync(cvSnapshot, jJdData);

                // 4. Cập nhật kết quả vào bản ghi JobApplication
                application.MatchScore = aiResult.MatchScore;
                application.AIAnalysis = aiResult.AIAnalysis;
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<bool> { IsSuccess = true, Message = "AI Analysis updated successfully." };
            }
            catch (Exception ex)
            {
                // Log lỗi ở đây
                return new ServiceResult<bool> { IsSuccess = false, Message = "AI Processing Error: " + ex.Message };
            }
        }

        //public async Task<ServiceResult<JobApplicationResponse>> CreateJobApplicationAsync(JobApplicationRequest request)
        //{
        //    try
        //    {
        //        var candidateIdString = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
        //        if (string.IsNullOrEmpty(candidateIdString) || !Guid.TryParse(candidateIdString, out var candidateId))
        //        {
        //            return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Invalid candidate ID from token." };
        //        }
        //        //validation request information
        //        if (request == null)
        //        {
        //            return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Invalid request data." };
        //        }  
        //        if   (request.postingResponse == null || request.postingResponse.isDeleted == true)
        //        {
        //            return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Job posting not found." };
        //        }
        //        //handle cv snapshot
        //        var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == request.CandidateProfileId,
        //            q => q.Include(c => c.Skills)
        //                  .Include(c => c.WorkExperiences)
        //                  .Include(c => c.Educations));
        //        //khong dc dung cv cua thang khac de apply
        //        if (cv == null || cv.CandidateId != candidateId)
        //        {
        //            return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Cv not found." };
        //        }
        //        var cvResponse = _mapper.Map<CVResponse>(cv);
        //        var jobApplication = _mapper.Map<JobApplication>(request);
        //        jobApplication.JobPostingId = request.postingResponse.Id;
        //        jobApplication.CandidateId = candidateId;
        //        jobApplication.ProfilesSnapshot = JsonSerializer.Serialize(cvResponse);

        //        //Handle Ai analysis and match score 
        //        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        //        var JDJsonString = JsonSerializer.Serialize(request.postingResponse);
        //        var prompt = $@"
        //        Bạn là một hệ thống ATS (Applicant Tracking System) chuyên nghiệp. Nhiệm vụ của bạn là đánh giá mức độ phù hợp giữa Hồ sơ ứng viên (CV) và Yêu cầu công việc (JD).

        //        [DỮ LIỆU YÊU CẦU CÔNG VIỆC (JD)]
        //        {JDJsonString}

        //        [DỮ LIỆU ỨNG VIÊN (CV)]
        //        {jobApplication.ProfilesSnapshot}

        //        TIÊU CHÍ ĐÁNH GIÁ (Thang điểm 0 - 100):
        //        1. Kỹ năng chuyên môn: CV có đáp ứng được các Requirement của JD không? (Nhận diện các từ đồng nghĩa, ví dụ: ReactJS = React, C# = .NET).
        //        2. Kinh nghiệm làm việc: Số năm kinh nghiệm thực tế (WorkExperiences) có khớp với YearsOfExperience của JD không?
        //        3. Trình độ học vấn & Chuyên môn: Có phù hợp với tính chất công việc không?

        //        YÊU CẦU ĐẦU RA BẮT BUỘC:
        //        Chỉ trả về định dạng JSON thuần túy (KHÔNG dùng thẻ markdown ```json...```), theo ĐÚNG cấu trúc sau:
        //        {{
        //        ""MatchScore"": 85.5,
        //        ""AIAnalysis"": ""{{ \""Strengths\"": [\""Kinh nghiệm 3 năm .NET\"", \""Có kỹ năng SQL\""], \""Weaknesses\"": [\""Thiếu kinh nghiệm làm việc với Redis\""], \""Summary\"": \""Ứng viên có nền tảng vững chắc, phù hợp 85% với vị trí này nhưng cần training thêm về Caching.\"" }}""
        //        }}
        //        Lưu ý: Trường ""AIAnalysis"" phải là một chuỗi string (đã được stringify từ một JSON object chứa Strengths, Weaknesses và Summary) để hệ thống có thể lưu trực tiếp vào database.
        //        ";
        //        var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
        //        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        //        // 1. Gửi Request lên Google
        //        var response = await _httpClient.PostAsync(url, jsonContent);
        //        var responseString = await response.Content.ReadAsStringAsync();

        //        // 2. Kiểm tra xem Google có báo lỗi (VD: sai API Key, hết Quota) không
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = $"Google API Error: {responseString}" };
        //        }

        //        // 3. BÓC LỚP VỎ JSON CỦA GOOGLE ĐỂ LẤY CÂU TRẢ LỜI CỦA AI
        //        using var googleDoc = JsonDocument.Parse(responseString);
        //        if (!googleDoc.RootElement.TryGetProperty("candidates", out var candidates))
        //        {
        //            return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Lỗi: Google không trả về kết quả (Có thể do safety filter chặn)." };
        //        }

        //        string aiRawText = candidates[0]
        //            .GetProperty("content")
        //            .GetProperty("parts")[0]
        //            .GetProperty("text")
        //            .GetString();

        //        // 4. DỌN DẸP KẾT QUẢ CỦA AI (Xóa bỏ các ký tự markdown ```json nếu AI ngoan cố thêm vào)
        //        if (!string.IsNullOrEmpty(aiRawText))
        //        {
        //            aiRawText = aiRawText.Replace("```json", "").Replace("```", "").Trim();
        //        }

        //        // 5. PARSE JSON THỰC SỰ DO AI TRẢ VỀ VÀ GÁN VÀO ENTITY
        //            using var aiDoc = JsonDocument.Parse(aiRawText);

        //            // Dùng TryGetProperty để chống lỗi KeyNotFoundException
        //            double directScore = 0;
        //            if (aiDoc.RootElement.TryGetProperty("MatchScore", out var scoreElement))
        //            {
        //                directScore = scoreElement.GetDouble();
        //            }
        //            // Phòng trường hợp AI viết chữ thường "matchScore"
        //            else if (aiDoc.RootElement.TryGetProperty("matchScore", out var lowerScoreElement))
        //            {
        //                directScore = lowerScoreElement.GetDouble();
        //            }

        //            string directAnalysis = string.Empty;
        //            if (aiDoc.RootElement.TryGetProperty("AIAnalysis", out var analysisElement))
        //            {
        //                // Nếu AIAnalysis là String thì lấy String, nếu AI lỡ trả về dạng Object JSON thì ép thành chuỗi Text
        //                directAnalysis = analysisElement.ValueKind == JsonValueKind.String
        //                    ? analysisElement.GetString()
        //                    : analysisElement.GetRawText();
        //            }

        //            // Gắn vào JobApplication
        //            jobApplication.MatchScore = directScore;
        //            jobApplication.AIAnalysis = directAnalysis;      
        //        await _unitOfWork.jobApplicationRepository.AddAsync(jobApplication);
        //        await _unitOfWork.SaveChangesAsync();
        //        return new ServiceResult<JobApplicationResponse> { IsSuccess = true, Data = _mapper.Map<JobApplicationResponse>(jobApplication) }; 

        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = ex.ToString() };
        //    }
        //}

        public async Task<ServiceResult<JobApplicationResponse>> CreateJobApplicationAsync(JobApplicationRequest request)
        {
            try
            {
                // 1. Lấy và kiểm tra CandidateId từ Token
                var candidateIdString = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(candidateIdString) || !Guid.TryParse(candidateIdString, out var candidateId))
                    return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Invalid candidate ID." };

                // 2. Kiểm tra dữ liệu đầu vào
                if (request?.postingResponse == null || request.postingResponse.isDeleted == true)
                    return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Job posting not found." };

                // 3. Lấy CV Snapshot
                var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == request.CandidateProfileId,
                    q => q.Include(c => c.Skills).Include(c => c.WorkExperiences).Include(c => c.Educations));

                if (cv == null || cv.CandidateId != candidateId)
                    return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "CV not found or access denied." };

                // 4. Map và lưu bản ghi
                var cvResponse = _mapper.Map<CVResponse>(cv);
                var jobApplication = _mapper.Map<JobApplication>(request);

                jobApplication.JobPostingId = request.postingResponse.Id;
                jobApplication.CandidateId = candidateId;
                jobApplication.ProfilesSnapshot = JsonSerializer.Serialize(cvResponse);

                // Mặc định ban đầu chưa có điểm AI
                jobApplication.MatchScore = 0;
                jobApplication.AIAnalysis = "Đang trong quá trình phân tích...";

                await _unitOfWork.jobApplicationRepository.AddAsync(jobApplication);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<JobApplicationResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<JobApplicationResponse>(jobApplication)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = ex.Message };
            }
        }
        public async Task<ServiceResult<string>> DeleteJobApplicationAsync(Guid id)
        {
            try
            {
                var jobApplication = await _unitOfWork.jobApplicationRepository.GetAsync(j => j.Id == id && !j.isDeleted);
                if (jobApplication == null)
                {
                    return new ServiceResult<string> { IsSuccess = false, Message = "Job application not found." };
                }
                await _unitOfWork.jobApplicationRepository.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string> { IsSuccess = true, Message = "Job application deleted successfully." };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<JobApplicationResponse>> GetJobApplicationByIdAsync(Guid id)
        {
            try
            {
                var jobApplication = await _unitOfWork.jobApplicationRepository.GetAsync(j => j.Id == id && !j.isDeleted);
                if (jobApplication == null)
                {
                    return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Job application not found." };
                }
                return new ServiceResult<JobApplicationResponse> { IsSuccess = true, Data = _mapper.Map<JobApplicationResponse>(jobApplication) };
            }
            catch (Exception ex)
            {
                return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<List<JobApplicationResponse>>> GetJobApplicationsByCandidateIdAsync(Guid candidateId)
        {
            try
            {
                var responses = new List<JobApplicationResponse>();
                var jobApplications = await _unitOfWork.jobApplicationRepository.GetAllAsync(j => j.CandidateId == candidateId && !j.isDeleted);
                if (jobApplications == null)
                {
                    return new ServiceResult<List<JobApplicationResponse>> { IsSuccess = false, Message = "No job applications found" };
                }
                foreach(var application in jobApplications)
                {
                    var resp = _mapper.Map<JobApplicationResponse>(application);
                    responses.Add(resp);
                }
                return new ServiceResult<List<JobApplicationResponse>> { IsSuccess = true, Data = responses };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<JobApplicationResponse>> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<List<JobApplicationResponse>>> GetJobApplicationsByJobPostingIdAsync(Guid jobPostingId)
        {
            try
            {
                var responses = new List<JobApplicationResponse>(); 
                var jobApplications = await _unitOfWork.jobApplicationRepository.GetAllAsync(j => j.JobPostingId == jobPostingId && !j.isDeleted);
                foreach(var application in jobApplications)
                {
                    var resp = _mapper.Map<JobApplicationResponse>(application);
                    responses.Add(resp);
                }
                return new ServiceResult<List<JobApplicationResponse>> { IsSuccess = true, Data = responses };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<JobApplicationResponse>> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<JobApplicationResponse>> UpdateJobApplicationAsync(Guid id, JobApplicationUpdateRequest request)
        {
            try
            {
                var jobApplication = await _unitOfWork.jobApplicationRepository.GetAsync(j => j.Id == id && !j.isDeleted);
                if (jobApplication == null)
                {
                    return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Job application not found." };
                }
                var oldCandidateProfileId = jobApplication.CandidateProfileId;
                _mapper.Map(request, jobApplication);
                //Nếu CandidateProfileId bị thay đổi, cần cập nhật lại ProfilesSnapshot và AI Analysis
                if (request.CandidateProfileId != oldCandidateProfileId)
                {
                    var candidateProfile = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == request.CandidateProfileId,
                        q => q.Include(c => c.Skills)
                              .Include(c => c.WorkExperiences)
                              .Include(c => c.Educations));
                    var jobPosting = await _unitOfWork.jobPostingRepository.GetAsync(j => j.Id == jobApplication.JobPostingId && !j.isDeleted);
                    if (jobPosting == null)
                    {
                        return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Job posting not found." };
                    }
                    var jobPostingResponse = _mapper.Map<JobPostingResponse>(jobPosting);
                    jobApplication.ProfilesSnapshot = JsonSerializer.Serialize(_mapper.Map<CVResponse>(candidateProfile));
                    var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
                    var JDJsonString = JsonSerializer.Serialize(jobPostingResponse);
                    var prompt = $@"
                Bạn là một hệ thống ATS (Applicant Tracking System) chuyên nghiệp. Nhiệm vụ của bạn là đánh giá mức độ phù hợp giữa Hồ sơ ứng viên (CV) và Yêu cầu công việc (JD).

                [DỮ LIỆU YÊU CẦU CÔNG VIỆC (JD)]
                {JDJsonString}

                [DỮ LIỆU ỨNG VIÊN (CV)]
                {jobApplication.ProfilesSnapshot}

                TIÊU CHÍ ĐÁNH GIÁ (Thang điểm 0 - 100):
                1. Kỹ năng chuyên môn: CV có đáp ứng được các Requirement của JD không? (Nhận diện các từ đồng nghĩa, ví dụ: ReactJS = React, C# = .NET).
                2. Kinh nghiệm làm việc: Số năm kinh nghiệm thực tế (WorkExperiences) có khớp với YearsOfExperience của JD không?
                3. Trình độ học vấn & Chuyên môn: Có phù hợp với tính chất công việc không?

                YÊU CẦU ĐẦU RA BẮT BUỘC:
                Chỉ trả về định dạng JSON thuần túy (KHÔNG dùng thẻ markdown ```json...```), theo ĐÚNG cấu trúc sau:
                {{
                ""MatchScore"": 85.5,
                ""AIAnalysis"": ""{{ \""Strengths\"": [\""Kinh nghiệm 3 năm .NET\"", \""Có kỹ năng SQL\""], \""Weaknesses\"": [\""Thiếu kinh nghiệm làm việc với Redis\""], \""Summary\"": \""Ứng viên có nền tảng vững chắc, phù hợp 85% với vị trí này nhưng cần training thêm về Caching.\"" }}""
                }}
                Lưu ý: Trường ""AIAnalysis"" phải là một chuỗi string (đã được stringify từ một JSON object chứa Strengths, Weaknesses và Summary) để hệ thống có thể lưu trực tiếp vào database.
                ";
                    var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                    // 1. Gửi Request lên Google
                    var response = await _httpClient.PostAsync(url, jsonContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    // 2. Kiểm tra xem Google có báo lỗi (VD: sai API Key, hết Quota) không
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = $"Google API Error: {responseString}" };
                    }

                    // 3. BÓC LỚP VỎ JSON CỦA GOOGLE ĐỂ LẤY CÂU TRẢ LỜI CỦA AI
                    using var googleDoc = JsonDocument.Parse(responseString);
                    if (!googleDoc.RootElement.TryGetProperty("candidates", out var candidates))
                    {
                        return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = "Lỗi: Google không trả về kết quả (Có thể do safety filter chặn)." };
                    }

                    string aiRawText = candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    // 4. DỌN DẸP KẾT QUẢ CỦA AI (Xóa bỏ các ký tự markdown ```json nếu AI ngoan cố thêm vào)
                    if (!string.IsNullOrEmpty(aiRawText))
                    {
                        aiRawText = aiRawText.Replace("```json", "").Replace("```", "").Trim();
                    }
                    // 5. PARSE JSON THỰC SỰ DO AI TRẢ VỀ VÀ GÁN VÀO ENTITY
                    using var aiDoc = JsonDocument.Parse(aiRawText);

                    // Dùng TryGetProperty để chống lỗi KeyNotFoundException
                    double directScore = 0;
                    if (aiDoc.RootElement.TryGetProperty("MatchScore", out var scoreElement))
                    {
                        directScore = scoreElement.GetDouble();
                    }
                    // Phòng trường hợp AI viết chữ thường "matchScore"
                    else if (aiDoc.RootElement.TryGetProperty("matchScore", out var lowerScoreElement))
                    {
                        directScore = lowerScoreElement.GetDouble();
                    }

                    string directAnalysis = string.Empty;
                    if (aiDoc.RootElement.TryGetProperty("AIAnalysis", out var analysisElement))
                    {
                        // Nếu AIAnalysis là String thì lấy String, nếu AI lỡ trả về dạng Object JSON thì ép thành chuỗi Text
                        directAnalysis = analysisElement.ValueKind == JsonValueKind.String
                            ? analysisElement.GetString()
                            : analysisElement.GetRawText();
                    }

                    // Gắn vào JobApplication
                    jobApplication.MatchScore = directScore;
                    jobApplication.AIAnalysis = directAnalysis;
                }
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<JobApplicationResponse> { IsSuccess = true, Data = _mapper.Map<JobApplicationResponse>(jobApplication) };

            }
            catch (Exception ex)
            {
                return new ServiceResult<JobApplicationResponse> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<string>> UpdateJobApplicationStatusAsync(Guid id, JobApplicationStatus status)
        {
            try
            {
                var jobApplication = await _unitOfWork.jobApplicationRepository.GetAsync(j => j.Id == id && !j.isDeleted);
                jobApplication.Status = status;
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string> { IsSuccess = true, Message = "Job application status updated successfully." };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string> { IsSuccess = false, Message = ex.Message };
            }
        }
    } 
}
