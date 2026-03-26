using System;
using System.Collections.Generic;

namespace EBCJobPortal.Models;

public partial class TblApplicant
{
    public int ApplyId { get; set; }

    public string? FullName { get; set; }

    public string? Gender { get; set; }

    public string? Nation { get; set; }
    public string? JobLocation { get; set; }
    public bool? AreYouDisable { get; set; }

    public string? ReasonForDisablity { get; set; }

    public int? Regid { get; set; }

    public string ZoneSubcity { get; set; } = null!;

    public string? Worede { get; set; }

    public string? HouseNumber { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MaritalStatus { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public int? JobId { get; set; }

    public string? CurrentWorkingCompany { get; set; }

    public string? PositionTitle { get; set; }

    public decimal? MonthlySalary { get; set; }

    public DateOnly? RequirementDate { get; set; }

    public string? CompanyPhoneNumber { get; set; }

    public string? CompanyPostNumber { get; set; }

    public string? IfNojobcurrently { get; set; }

    public string? ResignationationReson { get; set; }

    public DateOnly? ResignationDate { get; set; }

    public decimal? NumberofExprianceYears { get; set; }

    public string? EducationLevel { get; set; }

    public string? EducationField { get; set; }

    public string? Institution { get; set; }

    public string? GraduationYear { get; set; }

    public double? Cgpa { get; set; }

    public string? CvShorttermTranings { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Cvfile { get; set; }

    public virtual TblJobList? Job { get; set; }

    public virtual TblRegion? Reg { get; set; }
}
