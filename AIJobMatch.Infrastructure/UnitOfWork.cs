using AIJobMatch.Application;
using AIJobMatch.Application.IRepositories;
using AIJobMatch.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly AppDbContext _context;
        public IUserRepository userRepository { get; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            userRepository = new UserRepository(_context);
        }
    }
}
