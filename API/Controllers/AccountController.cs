using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.VisualBasic;
using SQLitePCL;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
         private readonly DataContext _context;
         private readonly ITokenService tokens;

        public AccountController(DataContext context,ITokenService tokenService)
        {
            _context = context;
            tokens = tokenService;
        }
          
          
          [HttpPost("register")]
                                                            
          public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){
             
             if(await UserExists(registerDto.username)){
                         
                      return BadRequest("username is  already taken");
             }
             using var hmac = new HMACSHA512();
              var user = new AppUser
             {
                UserName = registerDto.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt = hmac.Key
             };
               
               _context.Users.Add(user);
               await _context.SaveChangesAsync();

               

               return new UserDto{
                UserName = user.UserName,
                Token = tokens.CreateToken(user)
               };
          }
                                                   
        private async Task<bool> UserExists(string username){

          return await _context.Users.AnyAsync(x=>x.UserName ==username.ToLower());
        } 
        [HttpPost("login")]

        public async Task<ActionResult<UserDto>> login(LoginDto loginDto){
                     var user = await _context.Users
                     .Include(p=>p.Photos)
                     .SingleOrDefaultAsync(x => x.UserName == loginDto.username);
                     if(user == null) return Unauthorized();

                      using var hmac = new HMACSHA512(user.PasswordSalt);
                        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));   
                        for(int i =0;i<ComputeHash.Length; i++){
                          if(ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");

                        }
                         return new UserDto{
                UserName = user.UserName,
                Token = tokens.CreateToken(user),
                photoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
                };
        }
         
    }
}