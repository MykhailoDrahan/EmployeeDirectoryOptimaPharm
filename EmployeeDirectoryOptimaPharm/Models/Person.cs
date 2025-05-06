using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Models
{
    public class Person : BaseEntity
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [JsonIgnore]
        public List<Employee> Employees { get; set; }
        public PersonIdentityDocument PersonIdentityDocument { get; set; }
        [JsonIgnore]
        public Guid TaxIdentifierId { get; set; }
        public TaxIdentifier TaxIdentifier { get; set; }
        [JsonIgnore]
        public string FullName
        {
            get
            {
                string middleNameInitial = !string.IsNullOrEmpty(MiddleName) ? $"{MiddleName[0]}." : string.Empty;
                return $"{LastName} {FirstName[0]}.{middleNameInitial}";
            }
        }
    }
}
