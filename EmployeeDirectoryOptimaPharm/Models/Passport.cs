using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Models
{
    public class Passport : BaseEntity, IDocumentNumberProvider
    {
        public string Series { get; set; }
        public string Number { get; set; }

        public string GetDocumentNumber()
        {
            return $"{Series} {Number}";
        }
    }
}
