using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services,IConfiguration config){
         services.AddDbContext<DataContext>(opt =>  
{
    opt.UseSqlite(config.GetConnectionString("DefaultConnection"));

});
services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<ITokenService,TokenService>();
services.AddScoped<IUserRepository,UserRepository>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
services.AddScoped<IPhotoService,PhotoService>();
  return services;
        }
    }
}