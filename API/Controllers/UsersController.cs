using System.Security.Claims;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public IPhotoService _PhotoService { get; }


        public UsersController(IUserRepository userRepository,IMapper mapper,IPhotoService photoService)
        {
            _PhotoService = photoService;
         
            _mapper = mapper;
            _userRepository = userRepository;
        }
        
        
        [HttpGet]
        public  async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams){
                  

              var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var currentuser = await _userRepository.GetUserByUsernameAsync(username);

            userParams.CurrentUsername = currentuser.UserName;

              if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender = currentuser.Gender ==  "male" ? "female":"male";
              }

          var users =  await _userRepository.GetUserAsync(userParams);  
                 
               
        var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
        
         Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
         
        return Ok(usersToReturn);
            
        }

        
        [HttpGet("{username}")]
        public  async Task<ActionResult<MemberDto>> GetUser(string username){
            var user = await _userRepository.GetUserByUsernameAsync(username);
            return _mapper.Map<MemberDto>(user);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(memberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if(user == null) return NotFound();

            _mapper.Map(memberUpdateDto,user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("failed to update user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>>  AddPhoto(IFormFile file){

            var username= User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);
             if(user == null) return NotFound();

             var result = await _PhotoService.AddPhotoAsync(file);
             if(result.Error != null) return BadRequest(result.Error.Message);
             var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
             };
             if(user.Photos.Count ==0) photo.IsMain = true;
             user.Photos.Add(photo);
             if(await _userRepository.SaveAllAsync()){
                return CreatedAtAction(nameof(GetUser),new {username =user.UserName},_mapper.Map<PhotoDto>(photo));
             }
             return BadRequest("problem adding photo");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             var user = await _userRepository.GetUserByUsernameAsync(username);
             if(user ==null) return NotFound();
             var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);
             if(photo == null) return NotFound();
             if(photo.IsMain) return BadRequest("this is already your main photo");
             var currentMain = user.Photos.FirstOrDefault(x=>x.IsMain);
             if(currentMain !=null) currentMain.IsMain = false;
             photo.IsMain = true;
             if(await _userRepository.SaveAllAsync()) return NoContent();
             return BadRequest("problem setting main photo");

        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
             var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             var user = await _userRepository.GetUserByUsernameAsync(username);

             var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

             if(photo == null) return NotFound();
             if ( photo.IsMain) return BadRequest("you can not delete main photo");
             if(photo.PublicId != null){

                var result = await _PhotoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
             }

             user.Photos.Remove(photo);
             if( await _userRepository.SaveAllAsync()) return Ok();
             return BadRequest("problem deleting photo");
        }
    } 
}