using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Models
{
    interface IDocumentNumberProvider
    {
        string GetDocumentNumber();
    }
}
