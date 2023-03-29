using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
             var resultContext = await next();

             if(! resultContext.HttpContext.User.Identity.IsAuthenticated) return;
             
        
            
  
             var username = resultContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
             var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
             var user = await repo.GetUserByUsernameAsync(username);
             user.LastActive = DateTime.UtcNow;
             await repo.SaveAllAsync();
        }
    }
}