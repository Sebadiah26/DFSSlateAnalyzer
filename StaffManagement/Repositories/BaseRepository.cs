using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using StaffManagement.Data;
using StaffManagement.Models;
using StaffManagement.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace StaffManagement.Repositories
{
    public class MemoryCacheItemInfo
    {
        public string Key { get; private set; }
        public object CacheItemValue { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastUpdateUsage { get; private set; }
        public DateTime AbsoluteExpiry { get; private set; }
        public TimeSpan SlidingExpiry { get; private set; }

        public MemoryCacheItemInfo(string key, object cacheItemValue,
            DateTime created, DateTime lastUpdateUsage, DateTime absoluteExpiry,
            TimeSpan slidingExpiry)
        {
            this.Key = key;
            this.CacheItemValue = cacheItemValue;
            this.Created = created;
            this.LastUpdateUsage = lastUpdateUsage;
            this.AbsoluteExpiry = absoluteExpiry;
            this.SlidingExpiry = slidingExpiry;
        }
    }

    public class BaseRepository
    {
        protected readonly accountmanagementContext _db = null;
        protected readonly IAuditLoggerService _log = null;
        protected readonly IHttpContextAccessor _httpContextAccessor = null;
        protected AppUser _appUser = null;
        protected IMemoryCache _cache;
        protected CancellationTokenSource _resetCacheToken = new();

        public BaseRepository(accountmanagementContext context, IAuditLoggerService log, IHttpContextAccessor contextAccessor, IMemoryCache memoryCache, AppUser appUser)
        {
            _db = context;
            _log = log;
            _httpContextAccessor = contextAccessor;
            _cache = memoryCache;
            _appUser = appUser;

            //AppUser appuser = new AppUser(_httpContextAccessor);
            //_appUser = appuser;


            //_appUser.GetAppUserData(_httpContextAccessor.HttpContext);


        }

        public void SetCache(string name, SelectList list)
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));


            _cache.Set(name, list, options);

        }

        public void ClearCache()
        {
            _resetCacheToken.Cancel();
            _resetCacheToken.Dispose();
            _resetCacheToken = new CancellationTokenSource();


        }


        public MemoryCacheItemInfo[] GetCacheItemInfo(MemoryCache cache)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            FieldInfo field = typeof(MemoryCache).GetField("_stores", bindFlags);
            object[] cacheStores = (object[])field.GetValue(cache);

            List<MemoryCacheItemInfo> info = new List<MemoryCacheItemInfo>();

            foreach (object cacheStore in cacheStores)
            {
                Type cacheStoreType = cacheStore.GetType();

                FieldInfo lockField = cacheStoreType.GetField("_entriesLock", bindFlags);

                object lockObj = lockField.GetValue(cacheStore);

                lock (lockObj)
                {

                    FieldInfo entriesField = cacheStoreType.GetField("_entries", bindFlags);
                    Hashtable entriesCollection = (Hashtable)entriesField.GetValue(cacheStore);

                    foreach (DictionaryEntry cacheItemEntry in entriesCollection)
                    {
                        Type cacheItemValueType = cacheItemEntry.Value.GetType();

                        string key = (string)cacheItemEntry.Key.GetType().GetProperty("Key", bindFlags).GetValue(cacheItemEntry.Key);

                        PropertyInfo value = cacheItemValueType.GetProperty("Value", bindFlags);
                        PropertyInfo utcCreated = cacheItemValueType.GetProperty("UtcCreated", bindFlags);
                        PropertyInfo utcLastUpdateUsage = cacheItemValueType.GetProperty("UtcLastUpdateUsage", bindFlags);
                        PropertyInfo utcAbsoluteExpiry = cacheItemValueType.GetProperty("UtcAbsExp", bindFlags);
                        PropertyInfo utcSlidingExpiry = cacheItemValueType.GetProperty("SlidingExp", bindFlags);

                        MemoryCacheItemInfo mcii = new MemoryCacheItemInfo(
                            key,
                            value.GetValue(cacheItemEntry.Value),
                            ((DateTime)utcCreated.GetValue(cacheItemEntry.Value)).ToLocalTime(),
                            ((DateTime)utcLastUpdateUsage.GetValue(cacheItemEntry.Value)).ToLocalTime(),
                            ((DateTime)utcAbsoluteExpiry.GetValue(cacheItemEntry.Value)).ToLocalTime(),
                            ((TimeSpan)utcSlidingExpiry.GetValue(cacheItemEntry.Value))
                            );

                        info.Add(mcii);
                    }

                }
            }

            return info.ToArray();
        }


    }
}