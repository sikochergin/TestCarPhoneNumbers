using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Dependence
    {
        public Guid Id { get; set; }
        public Guid CarId { get; set; }
        public Guid PhoneId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsOwner { get; set; }
    }
}
