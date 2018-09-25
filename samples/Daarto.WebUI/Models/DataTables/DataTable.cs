namespace Daarto.Models
{
    public class DataTable
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Column[] Columns { get; set; }
        public Search Search { get; set; }
        public Order[] Order { get; set; }
    }
}
