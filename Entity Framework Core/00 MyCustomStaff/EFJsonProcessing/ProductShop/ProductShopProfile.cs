using AutoMapper;
using Newtonsoft.Json.Linq;
using ProductShop.Models;
using ProductShop.Models.Enumerators;
using ProductShop.ModelsDTO;
using ProductShop.ModelsDTO.GetSoldProducts;
using ProductShop.ModelsDTO.GetUsersAndProducts;
using System.Collections.Generic;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {

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
                .ForMember(x => x.SoldProducts, y => y.MapFrom(s => s.ProductsSold.Where(x => x.BuyerId != null)));

            //CategoriesByProductsCount
            this.CreateMap<Category, CategoriesByProductsCountDTO>()
                .ForMember(x => x.Category, y => y.MapFrom(s => s.Name))
                .ForMember(x => x.ProductsCount, y => y.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(x => x.AveragePrice, y => y
                    .MapFrom(s => s.CategoryProducts.Average(p => p.Product.Price).ToString("F2")))
                .ForMember(x => x.TotalRevenue, y => y
                    .MapFrom(s => s.CategoryProducts.Sum(p => p.Product.Price).ToString("F2")));

            //Products
            this.CreateMap<Product, ListOfProductsInRangeDTO>()
                .ForMember(x => x.SellerName, y =>
                    y.MapFrom(s => s.Seller.FirstName + " " + s.Seller.LastName));

            //UsersWithSoldedProducts
            //this.CreateMap<User, UsersWithSoldedProductsDTO>();

            this.CreateMap<User, SoldedProductsDTO>()
                .ForMember(x => x.Count, y => y.MapFrom(s => s.ProductsSold.Count(p => p.BuyerId != null)))
                .ForMember(x => x.ProductsSold, y => y.MapFrom(s => s.ProductsSold.Where(p => p.BuyerId != null)));

            this.CreateMap<UsersWithSoldedProductsDTO, UsersProductsDTO>()
                .ForMember(x => x.UsersCount, y => y.MapFrom(s => s.UserSoldedProducts.Count()))
                .ForMember(x => x.UsersProducts, y => y.MapFrom(s => s.UserSoldedProducts));

            this.CreateMap<SoldedProductsDTO, UsersProductsDTO>()
                .ForMember(x => x.UsersCount, y => y.MapFrom(s => s.Count))
                .ForMember(x => x.UsersProducts, y => y.MapFrom(s => s.ProductsSold));

            this.CreateMap<ListOfProductsInRangeDTO, UsersWithSoldedProductsDTO>();
        }
    }


}
