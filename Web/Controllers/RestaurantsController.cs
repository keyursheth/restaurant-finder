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

        public IActionResult GetRestaurantComponent(int entityId)
        {
            return ViewComponent("RestaurantList", entityId);
        }
    }
}