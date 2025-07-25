using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TestCarPhoneNumbers.Models
{
    public class NumberToShow
    {
        public string Number { get; set; }
        public DateTime CreationDate  { get; set; }
        public bool IsOwner { get; set; }
    }
}
