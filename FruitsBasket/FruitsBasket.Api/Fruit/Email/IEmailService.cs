using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Api.Fruit.Email;

public interface IEmailService
{
    Task SendFruitNotificationAsync(FruitDto fruit, string action);
}