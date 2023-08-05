namespace POC_WebCrawler.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Cpf { get; private set; }
        public List<string> RegisterNumber { get; private set; }

        public Customer(string cpf, List<string> registerNumber)
        {
            Cpf = cpf;
            RegisterNumber = registerNumber;
        }
    }
}
