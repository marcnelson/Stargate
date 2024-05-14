using Dapper;
using MediatR;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;
        public GetPersonByNameHandler(StargateContext context, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var result = new GetPersonByNameResult();

                var query = $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE '{request.Name}' = a.Name";

                var person = await _context.Connection.QueryAsync<PersonAstronaut>(query);

                result.Person = person.FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error handling get person by name for {RequestName}.", request?.Name ?? "Unknown");

                // Rethrow the exception to propagate it
                throw;
            }
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}
