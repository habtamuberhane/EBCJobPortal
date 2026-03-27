using System.ComponentModel.DataAnnotations;

namespace EBCJobPortalAdmin.ViewModel;

public sealed class AdminLoginViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool IsRemember { get; set; }
}
