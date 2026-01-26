using AIJobMatch.Application.IRepositories;
using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Repositories
{
    public class WorkExperienceRepository : GenericRepository<WorkExperiences>, IWorkExperienceRepository
    {
        public WorkExperienceRepository(AppDbContext context) : base(context)
        {
        }
    }
}
