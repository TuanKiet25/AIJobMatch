using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AIJobMatch.Application.Services
{
    public class AiCvService : IAiCvService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AiCvService(HttpClient httpClient, IConfiguration config, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            // Đọc Key từ file appsettings.json
            _apiKey = config["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("Thiếu cấu hình Gemini API Key");
            _model = config["GeminiAI:Model"] ?? "gemini-1.5-flash";
        }

        public async Task<ServiceResult<AiCvReviewResponse>> SuggestCvDataAsync(CVRequest cVRequest)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

            // 1. Biến cái CVRequest người dùng gửi lên thành chuỗi JSON để AI đọc
            string cvJsonString = JsonSerializer.Serialize(cVRequest);

            // 2. Viết Prompt yêu cầu AI làm giám khảo
            var prompt = $@"
        Bạn là một Headhunter cấp cao. Hãy review bản CV dưới đây (đang ở định dạng JSON) và đưa ra đánh giá, chấm điểm, cùng các gợi ý viết lại sao cho chuyên nghiệp hơn.

        DỮ LIỆU CV:
        {cvJsonString}

        YÊU CẦU OUTPUT: 
        Chỉ trả về JSON thuần túy theo đúng format sau (Không dùng markdown):
        {{
            ""Score"": 85,
            ""Strengths"": [""Ý 1"", ""Ý 2""],
            ""Weaknesses"": [""Ý 1"", ""Ý 2""],
            ""Suggestions"": [
                {{
                    ""Section"": ""WorkExperiences"",
                    ""ItemIndex"": 0,
                    ""SubSection"": ""Description"",
                    ""OriginalText"": ""Làm API cho web bán hàng."",
                    ""SuggestedText"": ""Thiết kế và phát triển RESTful API cho nền tảng E-commerce, tối ưu hóa database giúp giảm 30% thời gian phản hồi."",
                    ""Reason"": ""Cần thêm các động từ mạnh và số liệu cụ thể để làm nổi bật thành tích.""
                }}
            ]
        }}
    ";

            // 3. Đóng gói Body (Giống hệt hàm SuggestCvDataAsync)
            var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // 4. Gửi Request
                var response = await _httpClient.PostAsync(url, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // In toàn bộ lỗi ra Message để Frontend hoặc Swagger thấy được
                    return new ServiceResult<AiCvReviewResponse>
                    {
                        IsSuccess = false,
                        Message = $"API Google báo lỗi {response.StatusCode}. Chi tiết: {errorContent}"
                    };
                }

                var responseString = await response.Content.ReadAsStringAsync();

                // 5. Parse kết quả trả về
                using (var doc = JsonDocument.Parse(responseString))
                {
                    var candidates = doc.RootElement.GetProperty("candidates");
                    if (candidates.GetArrayLength() > 0)
                    {
                        var textResult = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                        // Dùng hàm CleanJsonString cũ để xóa ```json
                        string cleanJson = CleanJsonString(textResult);

                        // Ép chuỗi JSON AI trả về thành object AiCvReviewResponse
                        var reviewData = JsonSerializer.Deserialize<AiCvReviewResponse>(cleanJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new ServiceResult<AiCvReviewResponse> { IsSuccess = true, Data = reviewData };
                    }
                }
                return new ServiceResult<AiCvReviewResponse> { IsSuccess = false, Message = "AI không trả về kết quả" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<AiCvReviewResponse> { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult<AiCvReviewResponse>> SuggestCvDataByIdAsync(Guid id)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
            var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == id && !c.isDeleted, include:
                    q => q.Include(c => c.Skills)
                          .Include(c => c.WorkExperiences)
                          .Include(c => c.Educations));
            if (cv.isDeleted == true)
            {
                return new ServiceResult<AiCvReviewResponse> { IsSuccess = false, Message = "CV đã bị xóa" };
            }
            var cvResponse = _mapper.Map<CVResponse>(cv);
            // 1. Biến cái CVRequest người dùng gửi lên thành chuỗi JSON để AI đọc
            string cvJsonString = JsonSerializer.Serialize(cvResponse);

            // 2. Viết Prompt yêu cầu AI làm giám khảo
            var prompt = $@"
        Bạn là một Headhunter cấp cao. Hãy review bản CV dưới đây (đang ở định dạng JSON) và đưa ra đánh giá, chấm điểm, cùng các gợi ý viết lại sao cho chuyên nghiệp hơn.

        DỮ LIỆU CV:
        {cvJsonString}

        YÊU CẦU OUTPUT: 
        Chỉ trả về JSON thuần túy theo đúng format sau (Không dùng markdown):
        {{
            ""Score"": 85,
            ""Strengths"": [""Ý 1"", ""Ý 2""],
            ""Weaknesses"": [""Ý 1"", ""Ý 2""],
            ""Suggestions"": [
                {{
                    ""Section"": ""WorkExperiences"",
                    ""ItemIndex"": 0,
                    ""SubSection"": ""Description"",
                    ""OriginalText"": ""Làm API cho web bán hàng."",
                    ""SuggestedText"": ""Thiết kế và phát triển RESTful API cho nền tảng E-commerce, tối ưu hóa database giúp giảm 30% thời gian phản hồi."",
                    ""Reason"": ""Cần thêm các động từ mạnh và số liệu cụ thể để làm nổi bật thành tích.""
                }}
            ]
        }}
    ";

            // 3. Đóng gói Body (Giống hệt hàm SuggestCvDataAsync)
            var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // 4. Gửi Request
                var response = await _httpClient.PostAsync(url, jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // In toàn bộ lỗi ra Message để Frontend hoặc Swagger thấy được
                    return new ServiceResult<AiCvReviewResponse>
                    {
                        IsSuccess = false,
                        Message = $"API Google báo lỗi {response.StatusCode}. Chi tiết: {errorContent}"
                    };
                }

                var responseString = await response.Content.ReadAsStringAsync();

                // 5. Parse kết quả trả về
                using (var doc = JsonDocument.Parse(responseString))
                {
                    var candidates = doc.RootElement.GetProperty("candidates");
                    if (candidates.GetArrayLength() > 0)
                    {
                        var textResult = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                        // Dùng hàm CleanJsonString cũ để xóa ```json
                        string cleanJson = CleanJsonString(textResult);

                        // Ép chuỗi JSON AI trả về thành object AiCvReviewResponse
                        var reviewData = JsonSerializer.Deserialize<AiCvReviewResponse>(cleanJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new ServiceResult<AiCvReviewResponse> { IsSuccess = true, Data = reviewData };
                    }
                }
                return new ServiceResult<AiCvReviewResponse> { IsSuccess = false, Message = "AI không trả về kết quả" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<AiCvReviewResponse> { IsSuccess = false, Message = ex.Message };
            }
        }
        private string CleanJsonString(string aiOutput)

        {
            if (string.IsNullOrEmpty(aiOutput)) return "{}";
            // Xóa ```json ở đầu và ``` ở cuối
            aiOutput = aiOutput.Replace("```json", "").Replace("```", "").Trim();
            return aiOutput;

        }
    }
}
