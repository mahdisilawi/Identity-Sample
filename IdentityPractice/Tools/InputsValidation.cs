namespace IdentityPractice.Tools
{
    public class InputsValidation
    {
        public static bool IsValidIranianPhoneNumber(string phoneNumber)
        {
            // Iranian phone numbers start with a zero, followed by 10 digits.
            // The first two digits after the zero should be one of the following: 11, 21, 31, 41, 51, 61, 71, 81, 91
            var regex = new System.Text.RegularExpressions.Regex(@"^(?:0|98|\+98|\+980|0098|098|00980)?(9\d{9})$");

            return regex.IsMatch(phoneNumber); 
        }
    }
}
