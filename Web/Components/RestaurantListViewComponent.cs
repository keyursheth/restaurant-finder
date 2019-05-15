using CommonObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Models;

namespace Web.View_Components
{
    public class RestaurantListViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _clientFactory;
        public RestaurantListViewComponent(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int entityId)
        {
            RestaurantsModel restaurantsModel = new RestaurantsModel();
            restaurantsModel.Restaurants = new List<RestaurantDO>();

            ZomatoRestaurantDO zomatoRestaurantDO = new ZomatoRestaurantDO();
            zomatoRestaurantDO = await GetRestaurants(entityId);

            if (zomatoRestaurantDO != null && zomatoRestaurantDO.results_found > 0 && zomatoRestaurantDO.results_shown > 0
                && zomatoRestaurantDO.restaurants != null && zomatoRestaurantDO.restaurants.Length > 0)
            {
                foreach (var res in zomatoRestaurantDO.restaurants)
                {
                    var restaurant = res.restaurant;
                    RestaurantDO restaurant3 = new RestaurantDO
                    {
                        Address = new AddressDO()
                        {
                            Area = restaurant.location.locality,
                            City = restaurant.location.city,
                            Street1 = restaurant.location.address
                        },
                        CostForTwo = restaurant.average_cost_for_two,
                        Cuisines = restaurant.cuisines,
                        Name = restaurant.name,
                        Ratings = restaurant.user_rating.aggregate_rating,
                        ImagePath = restaurant.thumb,
                        VotesCount = restaurant.user_rating.votes
                    };

                    restaurantsModel.Restaurants.Add(restaurant3);
                }
            }

            return View(restaurantsModel);
        }

        private async Task<ZomatoRestaurantDO> GetRestaurants(int entityId)
        {
            ZomatoRestaurantDO zomatoRestaurantDO = new ZomatoRestaurantDO();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get,
                "https://developers.zomato.com/api/v2.1/search?entity_id=" + entityId + "&entity_type=city");
            requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Add("user-key", "06159f26379fc7fc1e8372033fe5ceff");

            var httpClient = _clientFactory.CreateClient();

            var response = await httpClient.SendAsync(requestMessage);

            if (response != null && response.IsSuccessStatusCode)
            {
                zomatoRestaurantDO = JsonConvert.DeserializeObject<ZomatoRestaurantDO>(response.Content.ReadAsStringAsync().Result);
            }

            return zomatoRestaurantDO;
        }
    }
}
