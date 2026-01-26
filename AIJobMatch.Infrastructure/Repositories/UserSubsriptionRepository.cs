using AIJobMatch.Application.IRepositories;
using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Repositories
{
    public class UserSubsriptionRepository : GenericRepository<UserSubscription>, IUserSubsriptionRepository
    {
        public UserSubsriptionRepository(AppDbContext context) : base(context)
        {
        }
    }
}
