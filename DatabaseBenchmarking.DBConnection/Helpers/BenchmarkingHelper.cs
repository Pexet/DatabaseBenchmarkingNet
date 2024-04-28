using System.ComponentModel;

namespace DatabaseBenchmarking.DBConnection.Helpers
{
    public static class BenchmarkingHelper
    {
        private static Random _random = new Random();

        public static string GenerateRandomString(int length)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string GetDescription(this Type type)
        {
            var descriptions = (DescriptionAttribute[])
                type.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptions.Length == 0)
            {
                return null;
            }
            return descriptions[0].Description;
        }

        public static string GetEmail()
        {
            return "random-email@gmail.com";
        }
    }
}
