using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<PagedList<AppUser>> GetUserAsync(UserParams userParams)
        {
          var query = _context.Users.AsQueryable();
          query = query.Where(u => u.UserName != userParams.CurrentUsername);
          query = query.Where(u => u.Gender == userParams.Gender);

          var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge -1));
          var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

          query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

          query = userParams.OrderBy switch 
          {
            "created" => query.OrderByDescending(u => u.Created),
            _=> query.OrderByDescending(u => u.LastActive)
          };
            

            return await PagedList<AppUser>.CreateAsync(query.Include(p=>p.Photos).AsNoTracking(),userParams.PageNumber,userParams.PageSize);
        
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}