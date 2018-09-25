using System.Collections.Generic;

namespace Daarto.Models
{
    public class DataTableResponse<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public int Draw { get; set; }
    }
}
