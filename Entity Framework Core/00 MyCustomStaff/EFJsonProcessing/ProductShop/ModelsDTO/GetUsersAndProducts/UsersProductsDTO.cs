using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.ModelsDTO.GetUsersAndProducts
{
    public class UsersProductsDTO
    {
        public int UsersCount { get; set; }

        public UsersWithSoldedProductsDTO[] UsersProducts { get; set; }
    }
}
