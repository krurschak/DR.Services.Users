using DR.Frameworks.Users.Models;
using DR.Packages.Mongo.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DR.Services.Users.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IMongoRepository<User> userRepository;

        public UsersController(
            ILogger<UsersController> logger,
            IMongoRepository<User> userRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
        }

        [HttpGet("{id}")]
        public async Task<Components.Users.Dto.User> Get([FromRoute] Guid id)
        {
            try
            {
                var user = await userRepository.GetAsync(id);

                if (user is null)
                {
                    logger.LogWarning($"user not found with id: {id}");
                    return null;
                }

                logger.LogInformation($"project found with id: {id}");
                return new Components.Users.Dto.User
                {
                    Id = user.Id,
                    Email = user.Email,
                    Salutation = user.Salutation,
                    Title = user.Title,
                    LastName = user.LastName,
                    FirstName = user.FirstName
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }

        [HttpGet("[action]")]
        public async Task<Components.Users.Dto.User> GetByIdentity([FromQuery] string sub, [FromQuery] string iss)
        {
            try
            {
                var user = await userRepository.GetAsync(x => x.IdentitySub == sub && x.IdentityIss == iss);

                if (user is null)
                {
                    return null;
                }

                return new Components.Users.Dto.User
                {
                    Id = user.Id,
                    Email = user.Email,
                    Salutation = user.Salutation,
                    Title = user.Title,
                    LastName = user.LastName,
                    FirstName = user.FirstName
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Components.Users.Dto.User>> Get([FromQuery] IEnumerable<Guid> ids = null)
        {
            try
            {
                if (ids is null || !ids.Any())
                {
                    logger.LogWarning("list of user ids cannot be null or empty");
                    return null;
                }

                var users = await userRepository.FindAsync(x => ids.Contains(x.Id));

                logger.LogInformation($"{users.Count()} projects found");
                return users.Select(x => new Components.Users.Dto.User
                {
                    Id = x.Id,
                    Email = x.Email,
                    Salutation = x.Salutation,
                    Title = x.Title,
                    LastName = x.LastName,
                    FirstName = x.FirstName
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }
    }
}
