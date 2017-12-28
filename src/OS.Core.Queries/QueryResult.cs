using System.Collections.Generic;

namespace OS.Core.Queries
{
    public class QueryResult<T>
    {
        public QueryResult()
        {
        }

        public QueryResult(IList<T> items, long filteredItems, long allItems)
        {
            ((List<T>)this.Items).AddRange(items);
            this.FilteredItems = filteredItems;
            this.AllItems = allItems;
        }

        public long AllItems { get; set; }
        public long FilteredItems { get; set; }
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
    }
}