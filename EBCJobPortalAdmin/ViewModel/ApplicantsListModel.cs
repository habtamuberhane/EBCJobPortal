using EBCJobPortalAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Query;

namespace EBCJobPortalAdmin.ViewModel
{
    public class ApplicantsListModel
    {
        public int? JobId { get; set; }
        public IEnumerable<SelectListItem>? Jobs { get; set; }
        public IQueryable<TblApplicant>? Applicants { get; set; }
    }
}
