using Dapper;
using MediatR;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;

        public GetAstronautDutiesByNameHandler(StargateContext context, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var result = new GetAstronautDutiesByNameResult();

                var query = $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE \'{request.Name}\' = a.Name";

                var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(query);

                if (person is null) throw new BadHttpRequestException("Bad Request");

                result.Person = person;

                query = $"SELECT * FROM [AstronautDuty] WHERE {person.PersonId} = PersonId Order By DutyStartDate Desc, Id Desc";

                var duties = await _context.Connection.QueryAsync<AstronautDuty>(query);

                if(!duties.Any()) throw new BadHttpRequestException("Bad Request");

                if(!duties.First().DutyTitle.Equals("RETIRED") && duties.First().DutyEndDate is not null) throw new BadHttpRequestException("Bad Request");

                result.AstronautDuties = duties.ToList();

                return result;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error handling astronaut duty for {RequestName}.", request?.Name ?? "Unknown");

                // Rethrow the exception to propagate it
                throw;
            }

        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}
