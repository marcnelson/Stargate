using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateAstronautDutyPreProcessor> _logger;
        public PersonController(IMediator mediator, ILogger<CreateAstronautDutyPreProcessor> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople()
                {

                });

                _logger.LogInformation("Getting people was successful");

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error getting people");

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return BadRequest("Name parameter is required.");

                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                _logger.LogInformation("Getting person was successful for {RequestName}.", name);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error getting person by name for {RequestName}.", name);

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) return BadRequest("Name parameter is required.");

                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
                });

                _logger.LogInformation("Creating person was successful for {RequestName}.", name);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                // Log exception
                _logger.LogError(ex, "Error creating person by name for {RequestName}.", name);

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }

        }
    }
}