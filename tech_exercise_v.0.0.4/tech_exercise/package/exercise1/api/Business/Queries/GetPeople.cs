using Dapper;
using MediatR;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        public readonly StargateContext _context;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;
        public GetPeopleHandler(StargateContext context, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var result = new GetPeopleResult();

                var query = $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id";

                var people = await _context.Connection.QueryAsync<PersonAstronaut>(query);

                result.People = people.ToList();

                return result;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error handling get people");

                // Rethrow the exception to propagate it
                throw;
            }
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };

    }
}
