using ResourceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceAPI.Entities;
using AutoMapper;

namespace ResourceAPI.Mappers
{
    public class UserContextProfile: Profile
    {
        public UserContextProfile()
        {
            //entity to model
            CreateMap<User, Models.User>(MemberList.Destination)
                .ForMember(x => x.Claims, opt => opt.MapFrom(src => src.Claims.Select(x => new Models.Claims(x.Type, x.Value))));

            //model to entity
            CreateMap<Models.User, User>(MemberList.Source)
                .ForMember(x => x.Claims,
                    opt => opt.MapFrom(src => src.Claims.Select(x => new Claims { Type = x.Type, Value = x.Value })));
        }
    }
}
