namespace EBCJobPortalAdmin.ViewModel;

public sealed class AdminDashboardViewModel
{
    public int TotalJobs { get; init; }

    public int TotalApplicants { get; init; }

    public int ActiveVacancies { get; init; }

    public bool IsDatabaseAvailable { get; init; } = true;

    public string? StatusMessage { get; init; }
}
