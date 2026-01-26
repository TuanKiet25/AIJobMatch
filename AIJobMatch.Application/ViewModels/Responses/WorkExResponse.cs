using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class WorkExResponse
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
