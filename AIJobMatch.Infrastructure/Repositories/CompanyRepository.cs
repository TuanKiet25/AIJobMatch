using AIJobMatch.Application.IRepositories;
using AIJobMatch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRegister
    {
        public CompanyRepository(AppDbContext context) : base(context)
        {
        }
    }
}
