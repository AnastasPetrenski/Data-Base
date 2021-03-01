using AutoMapper;
using Newtonsoft.Json.Linq;
using ProductShop.Models;
using ProductShop.Models.Enumerators;
using ProductShop.ModelsDTO;
using ProductShop.ModelsDTO.GetSoldProducts;
using System.Collections.Generic;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //Products
            this.CreateMap<Product, ListOfProductsInRangeDTO>()
                .ForMember(x => x.SellerName, y =>
                    y.MapFrom(s => s.Seller.FirstName + " " + s.Seller.LastName));

            //UserInfos
            this.CreateMap<JToken, UserInfo>()
                .ForMember(x => x.FirstName, y => y.MapFrom(j => j.SelectToken(".firstName")))
                .ForMember(x => x.LastName, y => y.MapFrom(j => j.SelectToken(".lastName")))
                .ForMember(x => x.Age, y => y.MapFrom(j => j.SelectToken(".age").Value<int?>() ?? null))
                .ForMember(x => x.AgeCategory, y => y.MapFrom(x => AgeCategory.NoCategory));

            //UserSoldProducts
            this.CreateMap<Product, UserSoldProductsDTO>()
                .ForMember(x => x.BuyerFirstName, y => y.MapFrom(s => s.Buyer.FirstName))
                .ForMember(x => x.BuyerLastName, y => y.MapFrom(s => s.Buyer.LastName));

            this.CreateMap<User, UserWithSoldProducts>()
                .ForMember(x => x.SoldProducts, y => y.MapFrom(s => s.ProductsSold));


        }
    }


}
