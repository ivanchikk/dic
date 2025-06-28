using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Infrastructure.RabbitMQ;

public record FruitEvent(FruitDto fruit, DateTime actionTime);