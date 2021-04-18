using DR.Components.Users.Commands;
using DR.Components.Users.Events;
using DR.Frameworks.Users.Models;
using DR.Packages.Mongo.Repository;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DR.Services.Users.Consumers.Commands
{
    public class SaveUserConsumer : IConsumer<SaveUser>
    {
        private readonly IMongoRepository<Event> eventRepository;
        private readonly IMongoRepository<User> userRepository;

        public SaveUserConsumer(
            IMongoRepository<Event> eventRepository,
            IMongoRepository<User> userRepository)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
        }

        public async Task Consume(ConsumeContext<SaveUser> context)
        {
            var user = await userRepository.GetAsync(context.Message.Id);

            if (user is null)
            {
                await eventRepository.AddAsync(new Event(NewId.NextGuid(), typeof(UserCreated).FullName, JsonConvert.SerializeObject(context.Message)));

                if (context.RequestId.HasValue)
                {
                    await context.RespondAsync<UserCreated>(context.Message);
                }

                await context.Publish<UserCreated>(context.Message);
            }
            else
            {
                await eventRepository.AddAsync(new Event(NewId.NextGuid(), typeof(UserUpdated).FullName, JsonConvert.SerializeObject(context.Message)));

                if (context.RequestId.HasValue)
                {
                    await context.RespondAsync<UserUpdated>(context.Message);
                }

                await context.Publish<UserUpdated>(context.Message);
            }
        }
    }
}
