using System.ComponentModel.DataAnnotations;

namespace IdentityPractice.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
