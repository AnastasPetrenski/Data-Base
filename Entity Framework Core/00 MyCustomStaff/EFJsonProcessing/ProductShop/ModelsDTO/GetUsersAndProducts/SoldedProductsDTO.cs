using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.ModelsDTO.GetUsersAndProducts
{
    public class SoldedProductsDTO
    {

        public int Count { get; set; }

        public ListOfProductsInRangeDTO[] ProductsSold { get; set; }
    }
}
