using System;
using System.Collections.Generic;
using System.Text;

namespace CommonObjects
{
    public class APIKeyNamesDO
    {
        public Apikeys apikeys { get; set; }
    }

    public class Apikeys
    {
        public string zomatokey { get; set; }
        public string gmapskey { get; set; }
    }
}
