using Microsoft.EntityFrameworkCore;
using Quarter.Models;
using X.PagedList;

namespace Quarter.Helpers
{
    public  class Pagination<T>:List<T>
    {

        public IPagedList<T> GetPagedNames(List<T> listUnpaged, int? page, int pageSize)
        {
            // return a 404 if user browses to before the first page
            if (page.HasValue && page < 1)
                return null;

            // page the list

            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);

            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;

            return listPaged;
        }
    }
}
