using System;
using System.Collections.Generic;

namespace EBCJobPortal.Models;

public partial class TblReference
{
    public int ReferencesId { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public int? ApplyId { get; set; }
}
