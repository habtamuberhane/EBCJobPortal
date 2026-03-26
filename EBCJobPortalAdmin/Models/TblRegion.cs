using System;
using System.Collections.Generic;

namespace EBCJobPortalAdmin.Models;

public partial class TblRegion
{
    public int Regid { get; set; }

    public string? RegionName { get; set; }

    public virtual ICollection<TblApplicant> TblApplicants { get; set; } = new List<TblApplicant>();
}
