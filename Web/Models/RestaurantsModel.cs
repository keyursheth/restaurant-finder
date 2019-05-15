using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommonObjects;

namespace Web.Models
{
    public class RestaurantsModel
    {
        public List<RestaurantDO> Restaurants { get; set; }
    }
}
