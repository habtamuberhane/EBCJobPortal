using EBCJobPortalAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EBCJobPortalAdmin.ViewModel
{
    public class ApplicantsListModel
    {
        public const int DefaultPageSize = 10;

        public int? JobId { get; set; }
        public int? Regid { get; set; }
        public string? Gender { get; set; }
        public string? DisabilityStatus { get; set; }
        public string? EducationLevel { get; set; }
        public double? MinimumCgpa { get; set; }
        public decimal? MinimumExperienceYears { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
        public int TotalApplicants { get; set; }
        public int TotalVacanciesCovered { get; set; }
        public int TotalDisabilityDisclosed { get; set; }
        public double AverageExperience { get; set; }
        public double AverageCgpa { get; set; }
        public IEnumerable<SelectListItem>? Jobs { get; set; }
        public IEnumerable<SelectListItem>? Regions { get; set; }
        public IEnumerable<SelectListItem>? Genders { get; set; }
        public IEnumerable<SelectListItem>? DisabilityStatuses { get; set; }
        public IEnumerable<SelectListItem>? EducationLevels { get; set; }
        public IReadOnlyList<TblApplicant> Applicants { get; set; } = [];

        public int TotalPages => TotalApplicants <= 0
            ? 1
            : (int)Math.Ceiling(TotalApplicants / (double)PageSize);

        public int StartItem => TotalApplicants == 0
            ? 0
            : ((PageNumber - 1) * PageSize) + 1;

        public int EndItem => Math.Min(PageNumber * PageSize, TotalApplicants);

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public IEnumerable<int> VisiblePageNumbers
        {
            get
            {
                var startPage = Math.Max(1, PageNumber - 2);
                var endPage = Math.Min(TotalPages, startPage + 4);

                if (endPage - startPage < 4)
                {
                    startPage = Math.Max(1, endPage - 4);
                }

                return Enumerable.Range(startPage, endPage - startPage + 1);
            }
        }
    }
}
