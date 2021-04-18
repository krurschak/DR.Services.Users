using DR.Components.Users.Events;
using DR.Frameworks.Users.Models;
using DR.Packages.Mongo.Repository;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DR.Services.Users.Consumers.Events
{
    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly IMongoRepository<User> userRepository;

        public UserCreatedConsumer(IMongoRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<UserCreated> context)
        {
            await userRepository.AddAsync(new User(
                context.Message.Id,
                context.Message.IdentitySub,
                context.Message.IdentityIss,
                context.Message.Email,
                context.Message.Salutation,
                context.Message.Title,
                context.Message.LastName,
                context.Message.FirstName));
        }
    }
}
