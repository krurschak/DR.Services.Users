using DR.Components.Projects.Events;
using DR.Components.Users.Events;
using MassTransit;
using System.Threading.Tasks;

namespace DR.Services.Users.Consumers.Events
{
    public class ProjectUserCreatedConsumer : IConsumer<ProjectUserCreated>
    {
        public async Task Consume(ConsumeContext<ProjectUserCreated> context)
        {
            await context.Publish<UserCreated>(new
            {
                Id = context.Message.UserId,
                Email = context.Message.UserEmail
            });
        }
    }
}
