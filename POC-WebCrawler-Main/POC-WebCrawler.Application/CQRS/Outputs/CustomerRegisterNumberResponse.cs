namespace POC_WebCrawler.Application.CQRS.Outputs
{
    public class CustomerRegisterNumberResponse
    {
        public IList<string> RegisterNumber { get; set; }

        public CustomerRegisterNumberResponse(IList<string> registerNumber)
        {
            RegisterNumber = registerNumber;
        }
    }
}
