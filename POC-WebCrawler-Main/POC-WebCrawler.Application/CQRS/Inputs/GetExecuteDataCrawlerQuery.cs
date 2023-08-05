using MediatR;
using POC_WebCrawler.Domain.Interfaces;

namespace POC_WebCrawler.Application.CQRS.Inputs
{
    public class GetExecuteDataCrawlerQuery : IRequest<int> { }

    public class GetExecuteDataCrawlerQueryHandler : IRequestHandler<GetExecuteDataCrawlerQuery, int>
    {
        private readonly IDataCrawlerService _crawlerService;
        public GetExecuteDataCrawlerQueryHandler(IDataCrawlerService crawlerService)
        {
            _crawlerService = crawlerService;
        }

        public async Task<int> Handle(GetExecuteDataCrawlerQuery request, CancellationToken cancellationToken)
        {
            return await _crawlerService.Execute();
        }
    }
}

