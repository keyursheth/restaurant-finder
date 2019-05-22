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
        public List<MapDetailsDO> MapDetails { get; set; }
        public string JsonMapDetails { get; set; }
        public string LocLatLong { get; set; }
        //public List<string> Cuisines { get; set; }
        public List<CuisinesDO> CuisineDetails { get; set; }

    }
}
