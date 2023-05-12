using Microsoft.Azure.Cosmos.Linq;

namespace SmartBlog.Infrastructure.Cosmos;

public static class Extentions
{
    public static async Task<IEnumerable<T>> ExecuteAsync<T>(this IQueryable<T> query)
    {
        var feedIterator = query.ToFeedIterator();
        using (feedIterator)
        {
            var itemList = new List<T>();

            while (feedIterator.HasMoreResults)
            {
                var items = await feedIterator.ReadNextAsync();
                itemList.AddRange(items);
            }

            return itemList;
        }
    }

    public static async Task<(string ContinuationToken, List<T> Items)> ExecutePagedAsync<T>(this IQueryable<T> query)
    {
        using var feedIterator = query.ToFeedIterator();

        if (feedIterator.HasMoreResults)
        {
            var items = await feedIterator.ReadNextAsync();

            return (items.ContinuationToken, items.ToList());
        }

        return (string.Empty, new List<T>());
    }
}
