using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser,MemberDto>()
            .ForMember(dest => dest.PhotoUrl, opt=>opt.MapFrom(src => src.Photos.FirstOrDefault(x=>x.IsMain).Url));
            CreateMap<Photo,PhotoDto>();
            CreateMap<memberUpdateDto,AppUser>();
            CreateMap<RegisterDto,AppUser>();
            CreateMap<Messages,MessageDto>()
            .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x =>x.IsMain).Url))
            .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x =>x.IsMain).Url));
        }
    }
}