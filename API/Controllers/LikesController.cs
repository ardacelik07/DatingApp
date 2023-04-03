using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Dtos;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        public LikesController(IUserRepository userRepository,ILikesRepository likesRepository)
        {
         _likesRepository = likesRepository;
          _userRepository = userRepository;
            
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
              var sourceUserId =int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
             var likedUser = await _userRepository.GetUserByUsernameAsync(username);
             var sourceUser= await _likesRepository.GetUserWithLikes(sourceUserId);

            if(likedUser == null) return NotFound();

            if(sourceUser.UserName == username) return BadRequest("you can not like yourself");

            var userLike = await _likesRepository.GetUserlike(sourceUserId,likedUser.Id);
            if(userLike != null) return BadRequest("you already like this user");

            userLike = new Entities.UserLike{
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("failed to like user");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams){

            likesParams.UserId =int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));

            return Ok(users);
        }
    }
}