using AutoMapper;
using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Data.Fruit;

public class FruitDaoProfile : Profile
{
    public FruitDaoProfile()
    {
        CreateMap<FruitDto, FruitDao>().ReverseMap();
    }
}