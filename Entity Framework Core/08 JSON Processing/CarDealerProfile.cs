using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using CarDealer.DTO;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportCarsDTO, Car>()
                .ForMember(x => x.Make, y => y.MapFrom(s => s.Make))
                .ForMember(x => x.Model, y => y.MapFrom(s => s.Model))
                .ForMember(x => x.TravelledDistance, y => y.MapFrom(s => s.TravelledDistance));

        }
    }
}
