using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommonObjects
{
    public class RestaurantDO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AddressDO Address { get; set; }
        public List<string> ContactNumbers { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Ratings { get; set; }

        public string Type { get; set; }
        public string Cuisines { get; set; }
        public int CostForTwo { get; set; }
        public string OperationHours { get; set; }

        public string ImagePath { get; set; }

        public string VotesCount { get; set; }
    }
}
