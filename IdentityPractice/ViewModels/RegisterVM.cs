using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityPractice.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MaxLength(250)]
        //[Remote("IsAnyUserName","Account",HttpMethod ="Post",AdditionalFields = "__RequestVerificationToken")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsAnyEmail", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Remote("IsPhoneValid","Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string Phone { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string RePassword { get; set; }
    }
}
