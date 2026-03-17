using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using AIJobMatch.Domain.Entities;
using AIJobMatch.Domain.Enums;
using AutoMapper;
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
    public class MockInterviewService : IMockInterviewService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MockInterviewService(HttpClient httpClient, IConfiguration config, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _apiKey = config["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("Thiếu cấu hình Gemini API Key");
            _model = config["GeminiAI:Model"] ?? "gemini-1.5-flash";
        }

        public async Task<ServiceResult<MockInterviewResponse>> ChatInterviewAsync(MockInterviewRequest request)
        {
            try
            {
                // 1. LẤY PHIÊN PHỎNG VẤN, LỊCH SỬ CÂU HỎI VÀ CV SNAPSHOT
                var interview = await _unitOfWork.mockInterviewRepository.GetAsync(
                    i => i.Id == request.MockInterviewId,
                    include: q => q.Include(i => i.Details.OrderBy(d => d.QuestionOrder))
                );

                if (interview == null)
                    return new ServiceResult<MockInterviewResponse> { IsSuccess = false, Message = "Không tìm thấy phiên phỏng vấn." };

                // 2. TÌM CÂU HỎI HIỆN TẠI VÀ CẬP NHẬT CÂU TRẢ LỜI CỦA ỨNG VIÊN
                var currentQuestion = interview.Details.FirstOrDefault(d => d.Id == request.MockInterviewDetailId);
                if (currentQuestion == null)
                    return new ServiceResult<MockInterviewResponse> { IsSuccess = false, Message = "Không tìm thấy câu hỏi tương ứng." };

                currentQuestion.CandidateAnswer = request.AnswerText;
                await _unitOfWork.mockInterviewDetailRepository.UpdateAsync(currentQuestion);

                // 3. KIỂM TRA ĐIỀU KIỆN KẾT THÚC (Giới hạn 5 câu hỏi)
                int maxQuestions = 5;
                if (interview.Details.Count >= maxQuestions)
                {
                    // Nếu đã trả lời xong câu số 5, lưu lại và báo FE biết đã kết thúc (để FE gọi API chấm điểm)
                    await _unitOfWork.SaveChangesAsync();
                    return new ServiceResult<MockInterviewResponse>
                    {
                        IsSuccess = true,
                        Data = new MockInterviewResponse
                        {
                            InterviewStatus = InterviewStatus.Completed,
                            NextQuestionId = null,
                            NextQuestionText = "Bạn đã hoàn thành buổi phỏng vấn thử! Hãy nhấn nút 'Xem Đánh Giá' để nhận phản hồi chi tiết từ AI.",
                            NextQuestionOrder = null
                        }
                    };
                }
                // 4. Gom toàn bộ lịch sử trò chuyện để AI hiểu ngữ cảnh
                var chatHistory = new StringBuilder();
                foreach (var detail in interview.Details.OrderBy(d => d.QuestionOrder))
                {
                    chatHistory.AppendLine($"HR: {detail.QuestionText}");
                    if (!string.IsNullOrEmpty(detail.CandidateAnswer))
                    {
                        chatHistory.AppendLine($"Ứng viên: {detail.CandidateAnswer}");
                    }
                }

                string jobTitle = interview.CustomTargetJobTitle ?? "Ứng viên";
                // Dịch mức độ khó thành chỉ thị cho AI
                string difficultyInstruction = interview.InterviewDifficulty switch
                {
                    InterviewDifficulty.easy => "Mức độ DỄ: Tập trung hỏi các khái niệm cơ bản, lý thuyết nền tảng. Thái độ thân thiện, mang tính chất kiểm tra kiến thức cơ bản.",
                    InterviewDifficulty.normal => "Mức độ TRUNG BÌNH: Tập trung hỏi về các tình huống thực tế, cách ứng dụng công nghệ/kỹ năng vào công việc. Thái độ chuyên nghiệp.",
                    InterviewDifficulty.hard => "Mức độ KHÓ: Hỏi xoáy sâu vào các chi tiết kỹ thuật, tối ưu hiệu năng, kiến trúc hệ thống (System Design) hoặc xử lý sự cố (Troubleshooting). Thái độ khắt khe, đưa ra các edge-case (trường hợp ngoại lệ) để thử thách ứng viên.",
                    _ => "Mức độ TRUNG BÌNH" // Mặc định
                };
                // 5. Viết Prompt "nhập vai" cho Gemini (Truyền cả CV Snapshot vào để AI hỏi xoáy vào CV)
                string prompt = $@"
                                Bạn là một Giám đốc Kỹ thuật đang phỏng vấn ứng viên cho vị trí: {jobTitle}.

                                Dưới đây là thông tin CV của ứng viên:
                                {interview.CvSnapshot}

                                Và đây là lịch sử cuộc trò chuyện từ đầu buổi đến giờ:
                                {chatHistory.ToString()}

                                Nhiệm vụ của bạn:
                                Dựa vào CV và câu trả lời cuối cùng của ứng viên, hãy đặt ra 1 câu hỏi tiếp theo (Câu số {interview.Details.Count + 1}).

                                YÊU CẦU QUAN TRỌNG VỀ ĐỘ KHÓ:
                                {difficultyInstruction}

                                Các quy tắc khác:
                                - Nối tiếp mạch trò chuyện hoặc đào sâu vào một điểm trong CV/câu trả lời trước.
                                - KHÔNG nhận xét, KHÔNG chấm điểm ở bước này. CHỈ đặt đúng 1 câu hỏi.

                                YÊU CẦU ĐẦU RA BẮT BUỘC (JSON thuần túy):
                                {{
                                 ""NextQuestionText"": ""Nội dung câu hỏi tiếp theo của bạn...""
                                }}";

                // 6. Cấu hình HTTP Request gửi sang Gemini
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new ServiceResult<MockInterviewResponse> { IsSuccess = false, Message = "Lỗi kết nối AI: " + responseString };

                // 7. Bóc tách JSON an toàn (Chống sập App)
                string nextQuestionText = "Đường truyền tín hiệu đang hơi kém, bạn có thể chia sẻ thêm về một dự án thực tế bạn tự hào nhất không?"; // Câu hỏi dự phòng (Fallback)

                try
                {
                    using var doc = JsonDocument.Parse(responseString);
                    var aiRawText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                    // Dọn dẹp markdown nếu AI lỡ sinh ra
                    if (!string.IsNullOrEmpty(aiRawText))
                    {
                        aiRawText = aiRawText.Replace("```json", "").Replace("```", "").Trim();
                        using var aiJson = JsonDocument.Parse(aiRawText);
                        if (aiJson.RootElement.TryGetProperty("NextQuestionText", out var qText))
                        {
                            nextQuestionText = qText.GetString() ?? nextQuestionText;
                        }
                    }
                }
                catch
                {
                    // Nếu AI nhả JSON lỗi, ta vẫn có câu "Fallback" ở trên để phỏng vấn không bị đứt đoạn!
                }

                // 8. TẠO VÀ LƯU CÂU HỎI MỚI VÀO DB
                var nextDetail = new MockInterviewDetail
                {
                    MockInterviewId = interview.Id,
                    QuestionOrder = interview.Details.Count + 1,
                    QuestionText = nextQuestionText,
                };

                await _unitOfWork.mockInterviewDetailRepository.AddAsync(nextDetail);

                // Lưu cả câu trả lời cũ + câu hỏi mới vào DB trong 1 Transaction
                await _unitOfWork.SaveChangesAsync();

                // 9. TRẢ KẾT QUẢ VỀ FRONTEND
                return new ServiceResult<MockInterviewResponse>
                {
                    IsSuccess = true,
                    Data = new MockInterviewResponse
                    {
                        InterviewStatus = interview.InterviewStatus,
                        NextQuestionId = nextDetail.Id,
                        NextQuestionText = nextDetail.QuestionText,
                        NextQuestionOrder = nextDetail.QuestionOrder
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<MockInterviewResponse> { IsSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<ServiceResult<MockInterviewResultResponse>> EvaluateInterviewAsync(Guid MockInterviewId)
        {
            try
            {
                // 1. Lấy toàn bộ dữ liệu buổi phỏng vấn
                var interview = await _unitOfWork.mockInterviewRepository.GetAsync(
                    i => i.Id == MockInterviewId,
                    include: q => q.Include(i => i.Details.OrderBy(d => d.QuestionOrder))
                );

                if (interview == null)
                    return new ServiceResult<MockInterviewResultResponse> { IsSuccess = false, Message = "Không tìm thấy phiên phỏng vấn." };

                // (Tùy chọn) Kiểm tra xem đã chấm điểm chưa để tránh tốn tiền gọi AI 2 lần
                if (interview.InterviewStatus == InterviewStatus.Evaluated)
                {
                    return new ServiceResult<MockInterviewResultResponse> { IsSuccess = true, Data = _mapper.Map<MockInterviewResultResponse>(interview) };
                }
                // 2. Tạo biên bản phỏng vấn (Transcript) để AI đọc
                var transcriptData = interview.Details.Select(d => new
                {
                    Order = d.QuestionOrder,
                    Question = d.QuestionText,
                    Answer = string.IsNullOrWhiteSpace(d.CandidateAnswer) ? "(Ứng viên bỏ qua không trả lời)" : d.CandidateAnswer
                });
                string transcriptJson = JsonSerializer.Serialize(transcriptData);

                string jobTitle = interview.CustomTargetJobTitle ?? "Ứng viên";

                // 3. Prompt siêu cấp ép buộc trả về JSON Schema
                string prompt = $@"
        Bạn là Hội đồng Tuyển dụng Đánh giá năng lực. Một ứng viên vừa hoàn thành bài phỏng vấn cho vị trí: {jobTitle}.
        
        Dưới đây là biên bản toàn bộ 5 câu hỏi và câu trả lời của ứng viên:
        {transcriptJson}

        Nhiệm vụ của bạn:
        1. Đọc kỹ từng câu trả lời.
        2. Chấm điểm cho từng câu (thang điểm 100) và đưa ra nhận xét chi tiết (chỉ ra điểm tốt, điểm sai, cách sửa) chú ý: nếu câu trả lời của ứng viên để trống thì mặc định điểm bằng 0.
        3. Đưa ra điểm tổng quát trung bình (OverallScore) tính bởi công thức trung bình cộng điểm của 5 câu trả lời và nhận xét tổng quan toàn bộ buổi phỏng vấn (OverallFeedback).
        
        BẮT BUỘC TRẢ VỀ ĐÚNG ĐỊNH DẠNG JSON SAU (KHÔNG DÙNG MARKDOWN):
        {{
            ""OverallScore"": 85.5,
            ""OverallFeedback"": ""Nhận xét tổng quan..."",
            ""Evaluations"": [
                {{
                    ""Order"": 1,
                    ""Score"": 90,
                    ""Feedback"": ""Nhận xét câu 1...""
                }},
                {{
                    ""Order"": 2,
                    ""Score"": 80,
                    ""Feedback"": ""Nhận xét câu 2...""
                }}
            ]
        }}";

                // 4. Gọi Gemini API
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
                var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new ServiceResult<MockInterviewResultResponse> { IsSuccess = false, Message = "Lỗi kết nối AI: " + responseString };

                // 5. Bóc tách JSON kết quả
                using var doc = JsonDocument.Parse(responseString);
                var aiRawText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                if (!string.IsNullOrEmpty(aiRawText))
                    aiRawText = aiRawText.Replace("```json", "").Replace("```", "").Trim();

                using var aiJson = JsonDocument.Parse(aiRawText);
                var root = aiJson.RootElement;

                // 6. Cập nhật dữ liệu vào Database
                interview.OverallScore = root.GetProperty("OverallScore").GetDouble();
                interview.OverallFeedback = root.GetProperty("OverallFeedback").GetString();
                interview.InterviewStatus = InterviewStatus.Evaluated;
                await _unitOfWork.mockInterviewRepository.UpdateAsync(interview);

                // Lặp qua mảng Evaluations do AI trả về để cập nhật điểm cho từng câu detail
                var evaluations = root.GetProperty("Evaluations").EnumerateArray();
                foreach (var eval in evaluations)
                {
                    int order = eval.GetProperty("Order").GetInt32();
                    var detailToUpdate = interview.Details.FirstOrDefault(d => d.QuestionOrder == order);

                    if (detailToUpdate != null)
                    {
                        detailToUpdate.QuestionScore = eval.GetProperty("Score").GetDouble();
                        detailToUpdate.AIFeedback = eval.GetProperty("Feedback").GetString();
                        await _unitOfWork.mockInterviewDetailRepository.UpdateAsync(detailToUpdate);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // 7. Trả về kết quả
                var mappedResult = _mapper.Map<MockInterviewResultResponse>(interview);
                return new ServiceResult<MockInterviewResultResponse> { IsSuccess = true, Data = mappedResult };
            }
            catch (Exception ex)
            {
                return new ServiceResult<MockInterviewResultResponse> { IsSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<ServiceResult<MockInterviewResultResponse>> GetMockInterviewResultAsync(Guid mockInterviewId)
        {
            try
            {
                var interview = await _unitOfWork.mockInterviewRepository.GetAsync(
                    i => i.Id == mockInterviewId,
                    include: q => q.Include(i => i.Details.OrderBy(d => d.QuestionOrder))
                );

                if (interview == null)
                    return new ServiceResult<MockInterviewResultResponse> { IsSuccess = false, Message = "Không tìm thấy dữ liệu." };

                var mappedResult = _mapper.Map<MockInterviewResultResponse>(interview);

                return new ServiceResult<MockInterviewResultResponse> { IsSuccess = true, Data = mappedResult };
            }
            catch (Exception ex)
            {
                return new ServiceResult<MockInterviewResultResponse> { IsSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<ServiceResult<StartInterviewResponse>> StartMockInterviewAsync(StartInterviewRequest request)
        {
            try
            {
                //handle cv snapshot
                var cv = await _unitOfWork.candidateProfileRepository.GetAsync(c => c.Id == request.CandidateProfileId,
                    q => q.Include(c => c.Skills)
                          .Include(c => c.WorkExperiences)
                          .Include(c => c.Educations));
                //khong dc dung cv cua thang khac de apply
                if (cv == null || cv.CandidateId != request.CandidateId)
                {
                    return new ServiceResult<StartInterviewResponse> { IsSuccess = false, Message = "Cv not found." };
                }

                // 2. Chốt chức danh phỏng vấn
                string jobTitle = string.IsNullOrWhiteSpace(request.CustomTargetJobTitle)
                    ? cv.DesiredJobTitle
                    : request.CustomTargetJobTitle;

                // 3. Tạo Snapshot CV (Đóng băng dữ liệu)
                string cvSnapshotJson = JsonSerializer.Serialize(_mapper.Map<CVResponse>(cv));

                // 4. TẠO BẢN GHI MASTER (MockInterview)
                var newInterview = new MockInterview
                {
                    CandidateId = request.CandidateId, // ID tài khoản đang login
                    CustomTargetJobTitle = jobTitle,
                    CvSnapshot = cvSnapshotJson,
                    InterviewDifficulty = request.InterviewDifficulty,
                    StartTime = DateTime.UtcNow,
                    InterviewStatus = InterviewStatus.InProgress,

                };

                await _unitOfWork.mockInterviewRepository.AddAsync(newInterview);
                await _unitOfWork.SaveChangesAsync(); // Cần Save trước để lấy được newInterview.Id

                // 5. TẠO CÂU HỎI SỐ 1 (Không cần gọi AI để tối ưu tốc độ)
                string firstQuestionText = $"Chào bạn, tôi là AI Interviewer. Rất vui được phỏng vấn bạn cho vị trí {jobTitle}. Trước tiên, bạn có thể giới thiệu ngắn gọn về bản thân và lý do bạn nghĩ mình phù hợp với vị trí này không?";

                var firstDetail = new MockInterviewDetail
                {
                    MockInterviewId = newInterview.Id,
                    QuestionOrder = 1,
                    QuestionText = firstQuestionText
                };

                await _unitOfWork.mockInterviewDetailRepository.AddAsync(firstDetail);
                await _unitOfWork.SaveChangesAsync(); // Lưu câu hỏi 1

                // 6. TRẢ KẾT QUẢ CHO FRONTEND BẮT ĐẦU CHAT
                return new ServiceResult<StartInterviewResponse>
                {
                    IsSuccess = true,
                    Data = new StartInterviewResponse
                    {
                        MockInterviewId = newInterview.Id,
                        FirstQuestionId = firstDetail.Id,
                        FirstQuestionText = firstQuestionText
                    }
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<StartInterviewResponse> { IsSuccess = false, Message = ex.ToString() };
            }
        }
    }
}
