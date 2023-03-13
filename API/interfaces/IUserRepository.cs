using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        
        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser>> GetUserAsync();

        Task<AppUser> GetUserByIdAsync(int id);

        Task<AppUser>  GetUserByUsernameAsync(string username);
    }
}