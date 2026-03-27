using EBCJobPortal.Models;

namespace EBCJobPortal.ViewModel;

public sealed class VacanciesIndexViewModel
{
    public const int DefaultPageSize = 6;

    public IReadOnlyList<TblJobList> Jobs { get; init; } = Array.Empty<TblJobList>();

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = DefaultPageSize;

    public int TotalJobs { get; init; }

    public int TotalPages => TotalJobs <= 0 ? 1 : (int)Math.Ceiling(TotalJobs / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public int StartItem => TotalJobs == 0 ? 0 : ((PageNumber - 1) * PageSize) + 1;

    public int EndItem => TotalJobs == 0 ? 0 : Math.Min(PageNumber * PageSize, TotalJobs);
}
