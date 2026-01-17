using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class LoginRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }
        public string CaptchaToken { get; set; }
    }
}
