using System.Collections.Generic;

namespace OS.Core.Queries
{
    public class Query
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 25;

        public IList<SortCriterium> SortCriteria { get; set; } = new List<SortCriterium>();

        public IList<FilterCriterium> FilterCriteria { get; set; } = new List<FilterCriterium>();
    }
}