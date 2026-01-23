using AIJobMatch.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string? PasswordHash { get; set; }
        public Role Role { get; set; }
        public List<Address>? Addresses { get; set; }
        public Candidate? Candidate { get; set; }
        public Recruiter? Recruiter { get; set; }
        public List<Transactions>? Transactions { get; set; }
        public List<UserSubscription>? UserSubscriptions { get; set; }
    }
}
