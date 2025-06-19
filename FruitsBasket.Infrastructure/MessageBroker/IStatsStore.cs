namespace FruitsBasket.Infrastructure.MessageBroker;

public interface IStatsStore<T>
{
    void Add(T id);
    IEnumerable<T> GetAll();
}