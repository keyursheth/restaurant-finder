using CommonObjects;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Web.Controllers
{
    public class RestaurantsController : Controller
    {        
        public RestaurantsController()
        {
            
        }
        public IActionResult Index()
        {           
            return View();
        }

        public IActionResult GetRestaurantComponent(int entityId, bool isFilterApplied, string cuisines)
        {
            return ViewComponent("RestaurantList", new { entityId = entityId, isFilterApplied = isFilterApplied, cuisines = cuisines });
        }
    }
}