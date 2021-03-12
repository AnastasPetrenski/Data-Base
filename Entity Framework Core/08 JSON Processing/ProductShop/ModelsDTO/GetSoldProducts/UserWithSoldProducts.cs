using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.ModelsDTO.GetSoldProducts
{
    public class UserWithSoldProducts
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [JsonProperty("soldProducts")]
        public UserSoldProductsDTO[] SoldProducts { get; set; }
    }
}
