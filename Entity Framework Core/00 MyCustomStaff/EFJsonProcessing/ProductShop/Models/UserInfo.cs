using Newtonsoft.Json;
using ProductShop.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("age")]
        public int? Age { get; set; }

        public AgeCategory AgeCategory { get; set; }
    }
}
