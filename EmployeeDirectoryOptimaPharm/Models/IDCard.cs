using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Models
{
    public class IDCard : BaseEntity, IDocumentNumberProvider
    {
        public string Number { get; set; }

        public string GetDocumentNumber()
        {
            return Number;
        }
    }
}
