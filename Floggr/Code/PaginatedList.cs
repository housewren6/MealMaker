//implements async ToPagedList
//from https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-7.0

namespace Floggr.Code
{
    public class PaginatedList<T> : List<T>
    {
        public List<T> Items { get; set; }
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            //int startIndex = (PageIndex - 1) * pageSize;
            //startIndex = Math.Max(0, startIndex);
            //startIndex = Math.Min(startIndex, items.Count - 1);
            //int itemsToTake = Math.Min(pageSize, count - startIndex);
            if (PageIndex >= TotalPages)
            {
                PageIndex = TotalPages;
            }
            Items = items.Skip((PageIndex - 1) * pageSize).Take(pageSize).ToList();
            //Items = items.Skip(startIndex).Take(itemsToTake).ToList();

        }
    }
}
