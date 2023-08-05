using MediatR;
using POC_WebCrawler.Application.CQRS.Outputs;
using POC_WebCrawler.Domain.Interfaces;

namespace POC_WebCrawler.Application.CQRS.Inputs
{
    public class GetRegisterNumberByCPFQuery : IRequest<CustomerRegisterNumberResponse>
    {
        public string CustomerCPF { get; private set; }
        public GetRegisterNumberByCPFQuery(string customerCPF)
        {
            CustomerCPF = customerCPF;
        }
    }

    public class GetRegisterNumberByCPFQueryHandler : IRequestHandler<GetRegisterNumberByCPFQuery, CustomerRegisterNumberResponse>
    {
        private readonly ICustomerService _customerService;
        public GetRegisterNumberByCPFQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<CustomerRegisterNumberResponse> Handle(GetRegisterNumberByCPFQuery request, CancellationToken cancellationToken)
        {
            var search = await _customerService.SearchCustomerDataByCpf(request.CustomerCPF);

            if (search == null || !search.Any())
                throw new Exception("Customer not found");

            return new CustomerRegisterNumberResponse(search.First().RegisterNumber);
        }
    }
}
