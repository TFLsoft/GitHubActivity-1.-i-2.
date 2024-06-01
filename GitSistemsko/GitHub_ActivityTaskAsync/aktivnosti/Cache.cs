using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubApi_ActivityTaskAsync.aktivnosti
{
    public static class Cache
    {
        private static Dictionary<string, CacheItem> cache = new Dictionary<string, CacheItem>();
        private static object lockObject = new object();

        public static List<GitHubEvent> GetFromCache(string key)
        {
            lock (lockObject)
            {
                if (cache.TryGetValue(key, out CacheItem cacheItem))
                {
                    if (DateTime.Now - cacheItem.Timestamp <= TimeSpan.FromMinutes(15))
                    {
                        return cacheItem.Data;
                    }
                    else
                    {
                        cache.Remove(key);
                    }
                }
            }
            return null;
        }

        public static void AddToCache(string key, List<GitHubEvent> data)
        {
            lock (lockObject)
            {
                var item = new CacheItem(data);
                cache[key] = item;
            }
        }

        public static void CleanupCache(object state)
        {
            lock (lockObject)
            {
                var keysToRemove = new List<string>();
                foreach (var item in cache)
                {
                    if (DateTime.Now - item.Value.Timestamp > TimeSpan.FromMinutes(15))
                    {
                        keysToRemove.Add(item.Key);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    cache.Remove(key);
                }
            }
        }
    }
}
