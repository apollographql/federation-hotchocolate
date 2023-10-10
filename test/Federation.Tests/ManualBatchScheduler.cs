using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using GreenDonut;

namespace ApolloGraphQL.Federation.HotChocolate;

// copied over from GreenDonut
public class ManualBatchScheduler : IBatchScheduler
{
    private readonly object _sync = new();
    private readonly ConcurrentQueue<Func<ValueTask>> _queue = new();

    public void Dispatch()
    {
        lock (_sync)
        {
            while (_queue.TryDequeue(out var dispatch))
            {
                dispatch();
            }
        }
    }

    public void Schedule(Func<ValueTask> dispatch)
    {
        lock (_sync)
        {
            _queue.Enqueue(dispatch);
        }
    }
}
