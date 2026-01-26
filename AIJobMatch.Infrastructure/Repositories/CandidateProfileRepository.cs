using AIJobMatch.Application.IRepositories;
using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Repositories
{
    public class CandidateProfileRepository : GenericRepository<CandidateProfile>, ICandidateProfileRepository
    {
        public CandidateProfileRepository(AppDbContext context) : base(context)
        {
        }
    }
}
