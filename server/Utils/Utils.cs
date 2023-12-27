using System.Text.RegularExpressions;
using System.Linq;

namespace server.Utils
{
    public class Utils
    {
        public static bool IsNumeric(string input)
        {
            return input.All(char.IsDigit);
        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
    }
}
