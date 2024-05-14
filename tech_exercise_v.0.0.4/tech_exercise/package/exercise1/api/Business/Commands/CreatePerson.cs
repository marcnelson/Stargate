using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;
        public CreatePersonPreProcessor(StargateContext context, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

                if (person is not null) throw new BadHttpRequestException("Bad Request");

                _logger.LogInformation("Astronaut duty processing successful for {RequestName}.", request.Name);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error processing person for {RequestName}.", request?.Name ?? "Unknown");

                // Rethrow the exception to propagate it
                throw;
            }
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly StargateContext _context;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;

        public CreatePersonHandler(StargateContext context, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var newPerson = new Person()
                {
                    Name = request.Name
                };

                await _context.People.AddAsync(newPerson);

                await _context.SaveChangesAsync();

                return new CreatePersonResult()
                {
                    Id = newPerson.Id
                };
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error handling person for {RequestName}.", request?.Name ?? "Unknown");

                // Rethrow the exception to propagate it
                throw;
            }

        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
