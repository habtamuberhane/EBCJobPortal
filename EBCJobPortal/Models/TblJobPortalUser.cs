using System;
using System.Collections.Generic;

namespace EBCJobPortal.Models;

public partial class TblJobPortalUser
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? FullName { get; set; }

    public string? PassWord { get; set; }

    public string? PhoneNumber { get; set; }

    public string? EmailAdress { get; set; }

    public bool? IsSuperAdmin { get; set; }
}
