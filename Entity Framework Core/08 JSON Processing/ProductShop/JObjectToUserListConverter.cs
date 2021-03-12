using AutoMapper;
using Newtonsoft.Json.Linq;
using ProductShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop
{
    public class JObjectToUserListConverter : ITypeConverter<JObject, List<User>>
    {
        public List<User> Convert(JObject source, List<User> destination, ResolutionContext context)
        {

            var userList = new List<User>();
            var tokenCountItems = source.SelectTokens("NameOfJObject").Children().Count();
            for (int i = 0; i < tokenCountItems; i++)
            {
                var token = source.SelectToken($"NameOfJObject[{i}]");
                var result = new User();

                if (token != null)
                {
                    result = context.Mapper.Map<JToken, User>(token);
                }
                userList.Add(result);
            }

            return userList;
        }   
    }
}

