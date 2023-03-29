using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserlike(int sourceUserId,int targetUserId);

        Task<AppUser>  GetUserWithLikes(int userId);
         
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams); 
    }
}