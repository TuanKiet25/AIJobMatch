using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public string? Industry { get; set; }
        public int? Size { get; set; }
        public string? Address { get; set; }
        public string TaxCode { get; set; }
        public string BusinessLicenseUrl { get; set; }
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime VerifiedAt { get; set; }

        public List<Recruiter>? Recruiters { get; set; }
    }
}
