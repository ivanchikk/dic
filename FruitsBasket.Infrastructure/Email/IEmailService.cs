using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Infrastructure.Email;

public interface IEmailService
{
    Task SendFruitNotificationAsync(FruitDto fruit, string action);
}