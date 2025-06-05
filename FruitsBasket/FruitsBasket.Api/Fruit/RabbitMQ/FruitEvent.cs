using FruitsBasket.Model.Fruit;

namespace FruitsBasket.Api.Fruit.RabbitMQ;

public record FruitEvent(FruitDto fruit, DateTime actionTime);