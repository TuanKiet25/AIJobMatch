using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Requests
{
    public class CompanyUpdateResquest
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public string? Industry { get; set; }
        public int? Size { get; set; }
        public AddressRequest Address { get; set; }
    }
}
