using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Ticketing_System_TILE03.Models;
using Ticketing_System_TILE03.Models.Domain;
namespace Ticketing_System_TILE03.Data.Repositories
{
    public class LoginAttemptRepository : ILoginAttemptRepository
    {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<LoginAttempt> _loginAttempts;

        public LoginAttemptRepository(ApplicationDbContext context)
        {
            _context = context;
            _loginAttempts = _context.LoginAttempts;
        }

        public void Add(LoginAttempt loginAttempt)
        {
            _loginAttempts.Add(loginAttempt);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}