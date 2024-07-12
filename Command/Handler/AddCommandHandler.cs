/*using Library.PublishSubscribe;
using MediatR;
using System.Threading.Tasks;
using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace Belajar2.Handler
{
    public class AddCommandHandler : AsyncRequestHandler<AddCommand>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPublishService<string, WeatherForecast> _pub;

        public AddCommandHandler(ApplicationDbContext dbContext, IPublishService<string, WeatherForecast> pub)
        {
            _pub = pub;
            _dbContext = dbContext;
        }

        protected override async Task Handle(AddCommand command, CancellationToken cancellationToken)
        {
            var user = new WeatherForecast
            {
                Id = command.Id,
                Password = command.Password,
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Address = command.Address
            };

            _dbContext.WeatherForecasts.Add(user);

            await _dbContext.SaveChangesAsync();

            await _bus.PublishAsync(command.Email, user);
        }
    }
}
*/