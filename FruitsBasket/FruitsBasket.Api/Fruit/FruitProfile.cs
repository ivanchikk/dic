using AutoMapper;
using FruitsBasket.Api.Fruit.Contract;
using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Api.Fruit;

public class FruitProfile : Profile
{
    public FruitProfile()
    {
        CreateMap<CreateFruit, FruitDto>().ReverseMap();
    }
}