using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class CompanyRegisterResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public string? Industry { get; set; }
        public int? Size { get; set; }
        public AddressResponse Address { get; set; }
        public string TaxCode { get; set; }
        public string BusinessLicenseUrl { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
