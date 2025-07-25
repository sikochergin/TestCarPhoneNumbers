using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Phone
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public DateTime CreationDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
