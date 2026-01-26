using AIJobMatch.Application.ViewModels.Requests;
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
    }
}
