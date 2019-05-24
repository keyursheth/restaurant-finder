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
        private readonly string _zomatoapikey;
        private readonly string _googleMapsKey;

        public RestaurantListViewComponent(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _zomatoapikey = SetZomatoAPIKey();
            _googleMapsKey = SetGoogleMapsKey();
        }

        public async Task<IViewComponentResult> InvokeAsync(int entityId, bool isFilterApplied, string cuisines)
        {            
            RestaurantsModel restaurantsModel = new RestaurantsModel();
            restaurantsModel.Restaurants = new List<RestaurantDO>();
            restaurantsModel.MapDetails = new List<MapDetailsDO>();
            List<string> lstIntersect = new List<string>();
            List<CuisinesDO> lstAllCuisines = new List<CuisinesDO>();
            //restaurantsModel.Cuisines = new List<string>();
            restaurantsModel.CuisineDetails = new List<CuisinesDO>();

            ZomatoRestaurantDO zomatoRestaurantDO = new ZomatoRestaurantDO();
            zomatoRestaurantDO = await GetRestaurants(entityId);
            restaurantsModel.LocLatLong = await GetLocationDetails(entityId);

            if (zomatoRestaurantDO != null && zomatoRestaurantDO.results_found > 0 && zomatoRestaurantDO.results_shown > 0
                && zomatoRestaurantDO.restaurants != null && zomatoRestaurantDO.restaurants.Length > 0)
            {
                List<string> lstFilterCuisines = new List<string>();

                if (string.IsNullOrEmpty(cuisines) == false)
                    lstFilterCuisines = cuisines.Split(',').ToList<string>();

                foreach (var res in zomatoRestaurantDO.restaurants)
                {
                    if (string.IsNullOrEmpty(res.restaurant.cuisines) == false)
                    {
                        var tempRestCuisines = res.restaurant.cuisines.Split(',');
                        List<string> lstResCuisines = new List<string>();

                        if (tempRestCuisines != null && tempRestCuisines.Count() > 0)
                        {
                            for (int i = 0; i < tempRestCuisines.Count(); i++)
                            {
                                lstResCuisines.Add(tempRestCuisines[i].Trim());
                            }
                        }

                        if (lstResCuisines != null && lstResCuisines.Count > 0)
                        {
                            //lstAllCuisines.AddRange(lstResCuisines);
                            foreach (var cuisine in lstResCuisines)
                            {
                                if (lstAllCuisines.Where(m => m.Name.ToLower().Trim() == cuisine.ToLower().Trim()).Count() > 0)
                                {
                                    lstAllCuisines.Where(m => m.Name.ToLower().Trim() == cuisine.ToLower().Trim())
                                                .First().Count += 1;
                                }
                                else
                                {
                                    lstAllCuisines.Add(new CuisinesDO() { Name = cuisine, Count = 1, IsChecked = false });
                                }                                
                            }

                            if (isFilterApplied)
                            {
                                lstFilterCuisines.ForEach(m => m.ToLower().TrimStart().TrimEnd());
                                lstResCuisines.ForEach(m => m.ToLower().TrimStart().TrimEnd());
                                lstIntersect = lstFilterCuisines.Intersect(lstResCuisines).ToList<string>();                                
                            }
                        }
                    }

                    if ((isFilterApplied && lstIntersect != null && lstIntersect.Count > 0)
                        || (isFilterApplied && lstFilterCuisines.Count == 0)
                        || isFilterApplied == false)
                    {
                        restaurantsModel.Restaurants.Add(GetRestaurantDO(res.restaurant));
                        restaurantsModel.MapDetails.Add(GetMapDetails(res.restaurant));
                    }                       
                }
                
                if (restaurantsModel.MapDetails.Count > 0)
                    restaurantsModel.JsonMapDetails = JsonConvert.SerializeObject(restaurantsModel.MapDetails);

                if (lstAllCuisines != null && lstAllCuisines.Count > 0)
                {
                    lstAllCuisines.ForEach(m => m.Name.Trim());
                    restaurantsModel.CuisineDetails.AddRange(lstAllCuisines.Distinct().OrderBy(m => m.Name));
                }

                if (isFilterApplied && lstAllCuisines != null && lstAllCuisines.Count > 0)
                {
                    foreach (var item in restaurantsModel.CuisineDetails)
                    {
                        bool check = false;
                        if (lstFilterCuisines.Where(m => m.ToLower().Trim() == item.Name.ToLower().Trim()).Count() > 0)
                        {
                            check = true;
                        }
                        
                        item.IsChecked = check;
                    }                    
                }
            }

            return View(restaurantsModel);
        }

        private async Task<ZomatoRestaurantDO> GetRestaurants(int entityId)
        {             
            ViewBag.gmapKey = _googleMapsKey;

            ZomatoRestaurantDO zomatoRestaurantDO = new ZomatoRestaurantDO();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get,
                "https://developers.zomato.com/api/v2.1/search?entity_id=" + entityId + "&entity_type=city");

            requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Add("user-key", _zomatoapikey);

            var httpClient = _clientFactory.CreateClient();

            var response = await httpClient.SendAsync(requestMessage);

            if (response != null && response.IsSuccessStatusCode)
            {
                zomatoRestaurantDO = JsonConvert.DeserializeObject<ZomatoRestaurantDO>(response.Content.ReadAsStringAsync().Result);
            }

            return zomatoRestaurantDO;
        }

        private RestaurantDO GetRestaurantDO(ZomatoRestaurant zomatoRestaurant)
        {
            RestaurantDO restaurant = new RestaurantDO
            {
                Address = new AddressDO()
                {
                    Area = zomatoRestaurant.location.locality,
                    City = zomatoRestaurant.location.city,
                    Street1 = zomatoRestaurant.location.address
                },
                CostForTwo = zomatoRestaurant.average_cost_for_two,
                Cuisines = zomatoRestaurant.cuisines,
                Name = zomatoRestaurant.name,
                Ratings = zomatoRestaurant.user_rating.aggregate_rating,
                ImagePath = zomatoRestaurant.thumb,
                VotesCount = zomatoRestaurant.user_rating.votes
            };

            return restaurant;
        }

        private MapDetailsDO GetMapDetails(ZomatoRestaurant zomatoRestaurant)
        {
            MapDetailsDO mapDetailsDO = new MapDetailsDO();

            mapDetailsDO.latitude = zomatoRestaurant.location.latitude;
            mapDetailsDO.longitude = zomatoRestaurant.location.longitude;
            mapDetailsDO.markerLabel = zomatoRestaurant.user_rating.aggregate_rating;

            return mapDetailsDO;
        }

        private async Task<string> GetLocationDetails(int entityId)
        {
            string latlong = string.Empty;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get,
                "https://developers.zomato.com/api/v2.1/location_details?entity_id=" + entityId + "&entity_type=city");

            requestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Add("user-key", _zomatoapikey);

            var httpClient = _clientFactory.CreateClient();

            var response = await httpClient.SendAsync(requestMessage);

            if (response != null && response.IsSuccessStatusCode)
            {                
                dynamic loc = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                latlong = loc.location.latitude + "," + loc.location.longitude;
            }

            return latlong;
        }

        private string SetZomatoAPIKey()
        {
            APIKeyNamesDO aPIKeyNamesDO = new APIKeyNamesDO();
            aPIKeyNamesDO = Utility.GetAPIKeyValue();

            return aPIKeyNamesDO.apikeys.zomatokey;
        }

        private string SetGoogleMapsKey()
        {
            APIKeyNamesDO aPIKeyNamesDO = new APIKeyNamesDO();
            aPIKeyNamesDO = Utility.GetAPIKeyValue();

            return aPIKeyNamesDO.apikeys.gmapskey;
        }
    }
}
