using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EBCJobPortalAdmin.ViewModel
{
    public class UserModel
    {
        public int UserId { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "*")]
        public string? UserName { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "New password and Confirmation password must match.")]
        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "First Name")]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "User Email")]
        public string? EmailAddress { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; }
        public bool IsRemember { get; set; }
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }


    }
}
