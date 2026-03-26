using System;
using System.Collections.Generic;

namespace EBCJobPortal.Models;

public partial class TblJobList
{
    public int JobId { get; set; }

    public int? RequiredNumber { get; set; }

    public DateTime? PostedDate { get; set; }

    public DateTime? ExpiredDate { get; set; }

    public string? JobTitle { get; set; }

    public string? JobDescription { get; set; }

    public virtual ICollection<TblApplicant> TblApplicants { get; set; } = new List<TblApplicant>();
}
