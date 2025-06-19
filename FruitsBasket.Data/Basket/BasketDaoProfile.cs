using AutoMapper;
using FruitsBasket.Model.Basket;

namespace FruitsBasket.Data.Basket;

public class BasketDaoProfile : Profile
{
    public BasketDaoProfile()
    {
        CreateMap<BasketDto, BasketDao>().ReverseMap();
    }
}