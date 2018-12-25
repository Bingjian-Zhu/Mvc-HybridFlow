using ResourceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResourceAPI.Entities;
using AutoMapper;

namespace ResourceAPI.Mappers
{
    public static class UserMappers
    {
        static UserMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserContextProfile>())
                .CreateMapper();
        }
        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.User ToModel(this User entity)
        {
            return Mapper.Map<Models.User>(entity);
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static User ToEntity(this Models.User model)
        {
            return Mapper.Map<User>(model);
        }
    }
}
