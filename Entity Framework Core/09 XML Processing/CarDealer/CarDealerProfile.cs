using AutoMapper;
using CarDealer.Models;
using CarDealer.Models.ExportsDtos;
using CarDealer.Models.ImportsDtos;
using System;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportCustomerDto, Customer>()
                .ForMember(x => x.BirthDate, y => y.MapFrom(s => DateTime.Parse(s.BirthDate)));

            this.CreateMap<ImportSaleDto, Sale>();

            this.CreateMap<Car, ExportCarDto>();

            this.CreateMap<Car, ExportBmwDto>();

            this.CreateMap<Supplier, ExportSupplierDto>()
                .ForMember(x => x.Count, y => y.MapFrom(s => s.Parts.Count));

            this.CreateMap<Car, ExportCarPartsDto>()
                .ForMember(x => x.Parts, y => y.MapFrom(s => s.PartCars.OrderByDescending(p => p.Part.Price)));

            this.CreateMap<PartCar, ExportCarPartsArrayDto>()
                .ForMember(x => x.Name, y => y.MapFrom(p => p.Part.Name))
                .ForMember(x => x.Price, y => y.MapFrom(p => p.Part.Price));

            this.CreateMap<Customer, ExportCustomerDto>()
                .ForMember(x => x.Count, y => y.MapFrom(s => s.Sales.Count))
                .ForMember(x => x.SpentMoney, y => y.MapFrom(s => s.Sales.Sum(c => c.Car.PartCars.Sum(w => w.Part.Price))));
        }
    }
}
