using System;
using System.Collections.Generic;
using System.Text;

namespace CommonObjects
{
    public class AddressDO
    {
        public int Id { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int PinCode { get; set; }
    }
}
