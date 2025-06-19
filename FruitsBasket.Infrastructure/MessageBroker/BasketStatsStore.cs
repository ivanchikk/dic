using System.Collections.Concurrent;

namespace FruitsBasket.Infrastructure.MessageBroker;

public class BasketStatsStore : IStatsStore<Guid>
{
    private readonly ConcurrentBag<Guid> _data = [];

    public void Add(Guid id)
    {
        _data.Add(id);
    }

    public IEnumerable<Guid> GetAll()
    {
        return _data.ToArray();
    }
}