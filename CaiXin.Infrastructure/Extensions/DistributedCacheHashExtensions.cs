using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Reflection;


namespace CaiXin.Infrastructure.Extensions;


/// <summary>
/// redis 扩展
/// </summary>
public static class DistributedCacheHashExtensions
{
    public static async Task SetHashAsync<T>(this IDistributedCache cache, ConnectionMultiplexer connectionMultiplexer, string key, T value) where T : class
    {
        var db = connectionMultiplexer.GetDatabase();

        var hashEntries = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(prop => new HashEntry(prop.Name, prop.GetValue(value)?.ToString() ?? ""))
            .ToArray();

        await db.HashSetAsync(key, hashEntries);
    }

    
    public static async Task<T> GetHash<T>(this IDistributedCache cache, ConnectionMultiplexer connectionMultiplexer, string key) where T : class, new()
    {
        var db = connectionMultiplexer.GetDatabase();
        var hashEntries = await db.HashGetAllAsync(key);
        var result = new T();
        foreach (var entry in hashEntries)
        {
            var property = typeof(T).GetProperty(entry.Name!);
            if (property != null && property.CanWrite)
            {
                var value = Convert.ChangeType(entry.Value.ToString(), property.PropertyType);
                property.SetValue(result, value);
            }
        }
        return result;
    }


    public static async Task<List<T>> GetHashDataByPatternAsync<T>(this IDistributedCache cache, ConnectionMultiplexer connectionMultiplexer, string pattern) where T : class, new()
    {
        var result = new List<T>();
        IDatabase db = connectionMultiplexer.GetDatabase();

        // 获取所有匹配的键
        var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints()[0]);
        var keys = new List<RedisKey>();
        foreach (var key in server.Keys(pattern: pattern))
        {
            keys.Add(key);
        }

        // 遍历键列表，获取每个键对应的哈希数据
        foreach (var key in keys)
        {
            var hashEntries = await db.HashGetAllAsync(key);
            var item = new T();

            foreach (var entry in hashEntries)
            {
                var property = typeof(T).GetProperty(entry.Name!);
                if (property != null && property.CanWrite)
                {
                     var value = Convert.ChangeType(entry.Value.ToString(), property.PropertyType);
                    property.SetValue(item, value);
                }
            }

            result.Add(item);
        }

        return result;
    }
}