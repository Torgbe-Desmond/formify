using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace FastTransfers.API.Controllers
{
    [Route("api/test")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>Testing.</summary>
        [HttpGet("")]
        public ActionResult<string> GetTest()
        {   
            var testing = "Yeah we are testing";
            return testing;
        }
    }
}
