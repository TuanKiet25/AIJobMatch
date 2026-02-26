using AIJobMatch.Application.IRepositories;
using AIJobMatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Repositories
{
    public class JobApplicationRepository : GenericRepository<JobApplication>, IJobApplicationRepository
    {
        public JobApplicationRepository(AppDbContext context) : base(context)
        {
        }

    }
}
