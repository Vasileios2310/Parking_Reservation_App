namespace ParkingReservationApp.Models;
/// <summary>
/// A paged subset of a collection of items of type T, plus pagination metadata.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PaginatedResult(IEnumerable<T> items, int pageNumber ,int pageSize,int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}