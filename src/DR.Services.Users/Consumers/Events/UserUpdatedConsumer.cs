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
    public class UserUpdatedConsumer : IConsumer<UserUpdated>
    {
        private readonly IMongoRepository<User> userRepository;

        public UserUpdatedConsumer(IMongoRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<UserUpdated> context)
        {
            var user = await userRepository.GetAsync(context.Message.Id);

            user.IdentitySub = context.Message.IdentitySub;
            user.IdentityIss = context.Message.IdentityIss;
            user.Email = context.Message.Email;
            user.Salutation = context.Message.Salutation ?? -9;
            user.Title = context.Message.Title;
            user.LastName = context.Message.LastName;
            user.FirstName = context.Message.FirstName;

            await userRepository.UpdateAsync(user);
        }
    }
}
