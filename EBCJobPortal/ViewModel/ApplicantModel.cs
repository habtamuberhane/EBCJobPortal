using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EBCJobPortal.ViewModel
{
    public class ApplicantModel
    {
        public int ApplyId { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "*")]
        public string? FullName { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "*")]
        public string? Gender { get; set; }

        public string[] Genders { get; set; } = ["Male", "Female"];

        [Display(Name = "Nation")]
        [Required(ErrorMessage = "*")]
        public string? Nation { get; set; }

        public IEnumerable<SelectListItem>? Nations { get; set; }

        [Required(ErrorMessage = "*")]
        public string? Disable { get; set; }

        public string[] Disablities { get; set; } = ["Yes", "No"];

        [Display(Name = "Region")]
        [Required(ErrorMessage = "*")]
        public int? Regid { get; set; }

        public IEnumerable<SelectListItem>? Regions { get; set; }

        [Display(Name = "Zone/Subcity")]
        [Required(ErrorMessage = "*")]
        public string ZoneSubcity { get; set; } = null!;

        [Display(Name = "Worede")]
        [Required(ErrorMessage = "*")]
        public string? Worede { get; set; }

        [Display(Name = "House number")]
        [Required(ErrorMessage = "*")]
        public string? HouseNumber { get; set; }

        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "*")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Marital status")]
        [Required(ErrorMessage = "*")]
        public string? MaritalStatus { get; set; }

        public string[] MaritalStatuses { get; set; } = ["Single", "Married", "Divorced", "Widowed"];

        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }

        public int? JobId { get; set; }

        public string? JobTitle { get; set; }

        [Display(Name = "Current or most recent employer")]
        public string? CurrentWorkingCompany { get; set; }

        [Display(Name = "Current or most recent job title")]
        public string? PositionTitle { get; set; }

        [Display(Name = "Current or most recent monthly salary")]
        public decimal? MonthlySalary { get; set; }

        public DateOnly? RequirementDate { get; set; }

        [Display(Name = "Company phone number")]
        public string? CompanyPhoneNumber { get; set; }

        [Display(Name = "Company post number")]
        public string? CompanyPostNumber { get; set; }

        public string? IfNojobcurrently { get; set; }

        public string? ResignationationReson { get; set; }

        public DateOnly? ResignationDate { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Years of experience")]
        public decimal? NumberofExprianceYears { get; set; }

        [Display(Name = "Education level")]
        [Required(ErrorMessage = "*")]
        public string? EducationLevel { get; set; }

        public IEnumerable<SelectListItem>? EducationLevels { get; set; }

        [Display(Name = "Area of study")]
        [Required(ErrorMessage = "*")]
        public string? EducationField { get; set; }

        [Display(Name = "Institution")]
        [Required(ErrorMessage = "*")]
        public string? Institution { get; set; }

        [Display(Name = "Graduation year")]
        [Required(ErrorMessage = "*")]
        public string? GraduationYear { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "CGPA")]
        public double? Cgpa { get; set; }

        [Display(Name = "Date of birth")]
        [Required(ErrorMessage = "*")]
        public DateOnly? BirthDate { get; set; }

        [Display(Name = "Resume")]
        public IFormFile? Cvfile { get; set; }
    }

    public class RegionModel
    {
        public string LocName { get; set; } = string.Empty;
    }
}
