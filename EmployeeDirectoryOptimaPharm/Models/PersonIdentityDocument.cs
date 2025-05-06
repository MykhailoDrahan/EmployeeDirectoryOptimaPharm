using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeDirectoryOptimaPharm.Models
{
    public enum IdentityDocumentType
    {
        IDCard,
        Passport
    }

    public class PersonIdentityDocument : BaseEntity
    {
        [JsonIgnore]
        public Guid PersonId { get; set; }
        [JsonIgnore]
        public Person Person { get; set; }
        public IdentityDocumentType Type { get; set; }
        [JsonIgnore]
        public Guid? IDCardId { get; set; }
        public IDCard? IDCard { get; set; }
        [JsonIgnore]
        public Guid? PassportId { get; set; }
        public Passport? Passport { get; set; }
        [JsonIgnore]
        public string DocumentNumber
        {
            get
            {
                switch(Type)
                {
                    case IdentityDocumentType.IDCard:
                        return IDCard.GetDocumentNumber();
                    case IdentityDocumentType.Passport:
                        return Passport.GetDocumentNumber();
                    default:
                        return "";
                }
            }
        }
        [JsonIgnore]
        public string DocumentName => Type.ToString();
    }
}
