using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Models
{
    public class Employee : BaseEntity
    {
        [JsonIgnore]
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
        [JsonIgnore]
        public Guid? PositionId { get; set; }
        public Position? Position { get; set; }   
        public decimal Salary { get; set; }
        public double EmploymentRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [JsonIgnore]
        public bool IsFinished => EndDate.HasValue && EndDate.Value.Date <= DateTime.Today;
    }
}
