using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class CreatePaymentRequest
    {
        [Required]
        public Guid PlanId { get; set; }

        public string? ReturnUrl { get; set; }

        public string? CancelUrl { get; set; }
    }
}
