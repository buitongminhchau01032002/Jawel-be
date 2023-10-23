using System.Data;

namespace Jawel_be.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public string Address { get; set; }
        public DataSetDateTime CreatedAt { get; set; }
        public string State { get; set; }
    }
}
