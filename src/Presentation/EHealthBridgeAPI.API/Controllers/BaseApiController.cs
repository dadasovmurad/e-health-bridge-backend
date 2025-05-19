using Results = Core.Results;
using Microsoft.AspNetCore.Mvc;
namespace EHealthBridgeAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetResponseResult(Results.IResult result)
        {
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}