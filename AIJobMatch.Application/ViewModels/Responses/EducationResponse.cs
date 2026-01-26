using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Application.ViewModels.Responses
{
    public class EducationResponse
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public string? SchoolName { get; set; }
        public string? Degree { get; set; }
        public string? Major { get; set; }
        public string? Grade { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
