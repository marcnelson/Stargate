using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;

        public CreateAstronautDutyPreProcessor(StargateContext context, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

                if (person is null) throw new BadHttpRequestException("Bad Request");

                var verifyNoPreviousDuty = _context.AstronautDuties.FirstOrDefault(z => 
                z.DutyTitle == request.DutyTitle && 
                z.DutyStartDate.Date == request.DutyStartDate.Date &&
                z.Rank == request.Rank);

                if (verifyNoPreviousDuty is not null) throw new BadHttpRequestException("Bad Request");

                var currentDuty = _context.AstronautDuties
                    .Where(z => z.PersonId == person.Id)
                    .OrderByDescending(z => z.DutyStartDate)
                    .ThenByDescending(z => z.Id)
                    .FirstOrDefault();

                if (currentDuty is not null && 
                    currentDuty.DutyTitle is not "RETIRED" && 
                    currentDuty.DutyEndDate is not null) throw new BadHttpRequestException("Bad Request");

                _logger.LogInformation("Astronaut duty processing successful for {RequestName}.", request.Name);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error processing astronaut duty for {RequestName}.", request?.Name ?? "Unknown");

                // Rethrow the exception to propagate it
                throw;
            }
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;

        public CreateAstronautDutyHandler(StargateContext context, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var query = $"SELECT * FROM [Person] WHERE \'{request.Name}\' = Name";

                var person = await _context.Connection.QueryFirstOrDefaultAsync<Person>(query);

                query = $"SELECT * FROM [AstronautDetail] WHERE {person.Id} = PersonId";

                var astronautDetail = await _context.Connection.QueryFirstOrDefaultAsync<AstronautDetail>(query);

                if (astronautDetail == null)
                {
                    astronautDetail = new AstronautDetail();
                    astronautDetail.PersonId = person.Id;
                    astronautDetail.CurrentDutyTitle = request.DutyTitle;
                    astronautDetail.CurrentRank = request.Rank;
                    astronautDetail.CareerStartDate = request.DutyStartDate.Date;
                    if (request.DutyTitle == "RETIRED")
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.Date;
                    }

                    await _context.AstronautDetails.AddAsync(astronautDetail);

                }
                else
                {
                    astronautDetail.CurrentDutyTitle = request.DutyTitle;
                    astronautDetail.CurrentRank = request.Rank;
                    if (request.DutyTitle == "RETIRED")
                    {
                        astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                    }
                    _context.AstronautDetails.Update(astronautDetail);
                }

                var astronautDuty = _context.AstronautDuties
                    .Where(z => z.PersonId == person.Id)
                    .OrderByDescending(z => z.DutyStartDate)
                    .ThenByDescending(z => z.Id)
                    .FirstOrDefault();

                if (astronautDuty != null)
                {
                    astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                    _context.AstronautDuties.Update(astronautDuty);
                }

                var newAstronautDuty = new AstronautDuty()
                {
                    PersonId = person.Id,
                    Rank = request.Rank,
                    DutyTitle = request.DutyTitle,
                    DutyStartDate = request.DutyStartDate.Date,
                    DutyEndDate = null
                };

                await _context.AstronautDuties.AddAsync(newAstronautDuty);

                await _context.SaveChangesAsync();

                return new CreateAstronautDutyResult()
                {
                    Id = newAstronautDuty.Id
                };
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error handling the astronaut duty creation for {RequestName}.", request?.Name ?? "Unknown");

                // Rethrow the exception to propagate it
                throw;
            }

        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
