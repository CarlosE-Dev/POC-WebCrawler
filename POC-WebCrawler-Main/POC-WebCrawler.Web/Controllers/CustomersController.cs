using MediatR;
using Microsoft.AspNetCore.Mvc;
using POC_WebCrawler.Application.CQRS.Inputs;
using POC_WebCrawler.Application.Validators;

namespace POC_WebCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/v1/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomersController(IMediator mediator)
         {
            _mediator = mediator;
        }

        [HttpGet("register/{cpf}")]
        public async Task<IActionResult> GetRegisterNumberByCPF([FromRoute] string cpf)
        {
            try
            {
                var request = new GetRegisterNumberByCPFQuery(cpf);
                var validator = await new GetRegisterNumberByCPFQueryValidator().ValidateAsync(request);

                if (!validator.IsValid)
                    return BadRequest(validator.Errors);

                return Ok(await _mediator.Send(request));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
