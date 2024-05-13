using Microsoft.AspNetCore.Mvc;

namespace StargateAPI.Controllers
{
    public static class ControllerBaseExtensions
    {

        public static IActionResult GetResponse(this ControllerBase controllerBase, BaseResponse response)
        {
            if (controllerBase is null) throw new ArgumentNullException(nameof(controllerBase), "Controller base cannot be null.");

            if (response is null) throw new ArgumentNullException(nameof(response), "Response object cannot be null.");

            var httpResponse = new ObjectResult(response);
            httpResponse.StatusCode = response.ResponseCode;
            return httpResponse;
        }
    }
}