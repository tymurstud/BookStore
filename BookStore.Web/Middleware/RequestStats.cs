using System.Collections.Concurrent;

namespace BookStore.Web.Middleware;

public class RequestStats
{
    private long _total;
    private readonly ConcurrentDictionary<string, long> _perPath = new();

    public long Total => Interlocked.Read(ref _total);

    public long Hit()
    {
        return Interlocked.Increment(ref _total);
    }

    public void TrackPath(string path)
    {
        _perPath.AddOrUpdate(path, 1, (_, v) => Interlocked.Increment(ref v));
    }

    public IReadOnlyList<(string Path, long Count)> Top5()
    {
        return _perPath
            .OrderByDescending(x => x.Value)
            .Take(5)
            .Select(x => (x.Key, x.Value))
            .ToList();
    }
}
