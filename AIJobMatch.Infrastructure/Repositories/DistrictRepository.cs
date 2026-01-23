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
    public class DistrictRepository : GenericRepository<District>, IDistrictRepository
    {
        public DistrictRepository(AppDbContext context) : base(context)
        {
        }

    }
}
