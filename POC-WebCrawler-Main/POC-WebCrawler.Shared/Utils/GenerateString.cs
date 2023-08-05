using System.Text;

namespace POC_WebCrawler.Shared.Utils
{
    public static class GenerateString
    {
        private static Random random = new Random();
        public static string RandomStringAlphaNumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return GetRandomCharSet(chars, length);
        }

        public static string RandomStringAlpha(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            return GetRandomCharSet(chars, length);
        }

        public static string RandomStringNumeric(int length)
        {
            const string chars = "1234567890";

            return GetRandomCharSet(chars, length);
        }

        private static string GetRandomCharSet(string chars, int length)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetCpfNumberMock()
        {
            var builder = new StringBuilder();
            builder.Append(RandomStringNumeric(3));
            builder.Append(".");
            builder.Append(RandomStringNumeric(3));
            builder.Append(".");
            builder.Append(RandomStringNumeric(3));
            builder.Append("-");
            builder.Append(RandomStringNumeric(2));

            return builder.ToString();
        }
    }
}
