using System;
using System.Runtime.Caching;

namespace ERP_V2
{
    public static class CacheHelper
    {
        private static ObjectCache Cache { get { return MemoryCache.Default; } }

        /// <summary> 取得Cache </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return Cache[key];
        }

        /// <summary> 移除Cache  </summary>
        /// <param name="key"></param>
        public static void Invalidate(string key)
        {
            Cache.Remove(key);
        }

        /// <summary> 設定 Cache 並且 設定 凌晨2點 回收 </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void Set(string key, object data)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Today.AddDays(1).AddHours(2);
            Cache.Add(new CacheItem(key, data), policy);
        }
    }
}