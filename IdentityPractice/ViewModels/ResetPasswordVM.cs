using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace IdentityPractice.ViewModels
{
    public class ResetPasswordVM
    {
        
        
        [Display(Name="New Password")]
        [System.ComponentModel.DataAnnotations.Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "New Password")]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Email { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Token { get; set; }

    }
}
