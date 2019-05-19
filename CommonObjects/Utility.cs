using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CommonObjects
{
    public static class Utility
    {
        static string ReadJsonFile()
        {
            string text = System.IO.File.ReadAllText(@"C:\Users\keyur\Desktop\GIT\restaurant-finder\Web\apikeys.json");
            return text;
        }

        public static APIKeyNamesDO GetAPIKeyValue()
        {
            APIKeyNamesDO aPIKeyNamesDO = new APIKeyNamesDO();

            string jsonData = ReadJsonFile();
            aPIKeyNamesDO = (APIKeyNamesDO)JsonConvert.DeserializeObject(jsonData, typeof(APIKeyNamesDO));

            return aPIKeyNamesDO;
        }
    }
}
