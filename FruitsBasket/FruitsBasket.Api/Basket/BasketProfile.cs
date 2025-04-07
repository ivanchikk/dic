using AutoMapper;
using FruitsBasket.Api.Basket.Contract;
using FruitsBasket.Model.Basket;

namespace FruitsBasket.Api.Basket;

public class BasketProfile : Profile
{
    public BasketProfile()
    {
        CreateMap<CreateBasket, BasketDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ReverseMap();
    }
}