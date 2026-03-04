using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Application.ViewModels.Responses;
using PayOS.Models.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.IServices
{
    public interface ITransactionService
    {
        Task<string> CreatePayment(CreatePaymentRequest request);
        Task<bool> VerifyWebhookSuccess(Webhook webhookData);
        Task<ServiceResult<string>> ApplySubscription(Guid planId);
    }
}
