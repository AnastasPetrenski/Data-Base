using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.ModelsDTO.GetUsersAndProducts
{
    public class UsersWithSoldedProductsDTO
    {
        public string LastName { get; set; }

        public int? Age { get; set; }

        public SoldedProductsDTO[] UserSoldedProducts { get; set; }
    }
}
