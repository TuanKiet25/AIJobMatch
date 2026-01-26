using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Entities;
using AIJobMatch.Domain.Enums;
using Microsoft.AspNetCore.Http;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AIJobMatch.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOSClient _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionService(IUnitOfWork unitOfWork, PayOSClient payOS, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> CreatePayment(CreatePaymentRequest request)
        {
            try
            {
                // 1. Lấy UserId từ token
                var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    throw new Exception("Invalid user ID from token");
                }

                // 2. Kiểm tra và lấy thông tin subscription plan
                var subscriptionPlan = await _unitOfWork.subscriptionPlansRepository.GetByIdAsync(request.PlanId);
                if (subscriptionPlan == null)
                {
                    throw new Exception("Subscription plan not found");
                }

                if (subscriptionPlan.isDeleted)
                {
                    throw new Exception("Subscription plan is deleted");
                }

                // 3. Tạo mã giao dịch duy nhất (sử dụng timestamp + random)
                var orderCode = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % int.MaxValue);
                var transactionCode = $"TXN_{orderCode}_{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

                // 4. Tạo transaction record với trạng thái Pending
                var transaction = new Transactions
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PlanId = request.PlanId,
                    Amount = subscriptionPlan.Price,
                    PaymentMethod = PaymentMethod.Banking, // Mặc định là Banking cho PayOS
                    TransactionCode = orderCode,
                    TransactionStatus = TransactionStatus.Pending,
                    CreateTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow,
                    isDeleted = false
                };

                // 5. Tạo payment link với PayOS
                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: (int)(subscriptionPlan.Price), // Chuyển đổi sang VND (số tiền nhỏ nhất)
                    description: $"Transaction for {subscriptionPlan.Name}",
                    returnUrl: request.ReturnUrl,
                    cancelUrl: request.CancelUrl
                );

                var paymentLinkResponse = await _payOS.PaymentRequests.CreateAsync(paymentData);

                // 6. Lưu transaction vào database
                await _unitOfWork.transactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                // 7. Trả về payment link
                return paymentLinkResponse.CheckoutUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating payment: {ex.Message}", ex);
            }
        }

        public async Task<bool> VerifyWebhookSuccess(Webhook webhookData)
        {
            try
            {
                var verifiedData = await _payOS.Webhooks.VerifyAsync(webhookData);
                long payOSOrderCode = verifiedData.OrderCode;

                var transaction = await _unitOfWork.transactionRepository
                    .GetAsync(x => x.TransactionCode == payOSOrderCode);

                if (transaction == null)
                {
                    throw new Exception($"Không tìm thấy giao dịch với mã: {payOSOrderCode}");
                }
                transaction.TransactionStatus = TransactionStatus.Completed;
                transaction.UpdateTime = DateTime.UtcNow;
                var userId = transaction.UserId;
                await _unitOfWork.transactionRepository.UpdateAsync(transaction);

                var userSubscription = new UserSubscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PlanId = transaction.PlanId,
                    Status = UserSubscriptionStatus.Active,
                    CreateTime = DateTime.UtcNow,
                    UpdateTime = DateTime.UtcNow,
                    isDeleted = false
                };
                await _unitOfWork.userSubsriptionRepository.AddAsync(userSubscription);

                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error handling webhook: {ex.Message}", ex);
            }
        }


        internal class PaymentData : CreatePaymentLinkRequest
        {
            public PaymentData(int orderCode, int amount, string description, string? returnUrl, string? cancelUrl)
            {
                OrderCode = orderCode;
                Amount = amount;
                Description = description;
                ReturnUrl = returnUrl;
                CancelUrl = cancelUrl;
            }
        }
    }
}