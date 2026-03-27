using EBCJobPortal.Models;

namespace EBCJobPortal.ViewModel;

public sealed class HomeIndexViewModel
{
    public int ActiveVacancyCount { get; init; }

    public IReadOnlyList<TblJobList> FeaturedJobs { get; init; } = Array.Empty<TblJobList>();
}
