using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;
        public AstronautDutyController(IMediator mediator, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return BadRequest("Name parameter is required.");

                var result = await _mediator.Send(new GetAstronautDutiesByName()
                {
                    Name = name
                });

                _logger.LogInformation("Get astronaut duties by name was successfully handled for {RequestName}.", name);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error handling astronaut duty for {RequestName}.", name ?? "Unknown");

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }            
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
        {
            try
            {
                if (request is null) throw new ArgumentNullException(nameof(request), "Request object cannot be null.");

                var result = await _mediator.Send(request);

                _logger.LogInformation("Creating astronaut duty by name was successfully handled for {RequestName}.", request.Name);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error creating astronaut duty for {RequestName}.", request?.Name ?? "Unknown");

                // Rethrow the exception to propagate it
                throw;
            }
        }
    }
}