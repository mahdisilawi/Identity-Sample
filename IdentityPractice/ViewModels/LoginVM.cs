using System.ComponentModel.DataAnnotations;

namespace IdentityPractice.ViewModels
{
    public class LoginVM
    {
        [Required]
        [Display(Name ="UserName")]
        [StringLength(200)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
