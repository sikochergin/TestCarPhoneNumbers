using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Phone {  get; set; }
        public Guid PhoneId { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
