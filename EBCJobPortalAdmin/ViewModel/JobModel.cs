namespace EBCJobPortalAdmin.ViewModel
{
    public class JobModel
    {
        public int JobId { get; set; }

        public int? RequiredNumber { get; set; }

        public DateTime? PostedDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public string? JobTitle { get; set; }

        public string? JobDescription { get; set; }
    }
}
