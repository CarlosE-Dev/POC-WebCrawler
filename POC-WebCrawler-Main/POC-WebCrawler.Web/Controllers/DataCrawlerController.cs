using MediatR;
using Microsoft.AspNetCore.Mvc;
using POC_WebCrawler.Application.CQRS.Inputs;

namespace POC_WebCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/v1/crawler")]
    public class DataCrawlerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DataCrawlerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> Execute()
        {
            try
            {
                var result = await _mediator.Send(new GetExecuteDataCrawlerQuery());
                return Ok($"{result} new register(s) found");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
