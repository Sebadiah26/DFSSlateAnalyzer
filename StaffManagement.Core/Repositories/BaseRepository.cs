using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using StaffManagement.Core.Services;
using StaffManagement.Data;
using System.Collections;
using System.Reflection;

namespace StaffManagement.Core.Repositories
{
    public class MemoryCacheItemInfo
    {
        public string Key { get; }
        public object CacheItemValue { get; }
        public DateTime Created { get; }
        public DateTime LastUpdateUsage { get; }
        public DateTime AbsoluteExpiry { get; }
        public TimeSpan SlidingExpiry { get; }

        public MemoryCacheItemInfo(string key, object cacheItemValue,
            DateTime created, DateTime lastUpdateUsage, DateTime absoluteExpiry,
            TimeSpan slidingExpiry)
        {
            Key = key;
            CacheItemValue = cacheItemValue;
            Created = created;
            LastUpdateUsage = lastUpdateUsage;
            AbsoluteExpiry = absoluteExpiry;
            SlidingExpiry = slidingExpiry;
        }
    }

    /// <summary>Ported from StaffManagement/Repositories/BaseRepository.cs. AppUser -&gt; ICurrentUserContext
    /// and IHttpContextAccessor is dropped (nothing in the legacy BaseRepository actually used it beyond
    /// passing it through) - otherwise unchanged, including the 5-minute sliding cache mechanism used by
    /// e.g. the ported EmployeeFilter unit list.
    ///
    /// Exposes _dbFactory rather than a shared _db field: derived repositories must create a
    /// short-lived context per method (await using var db = await _dbFactory.CreateDbContextAsync();)
    /// rather than holding one for the repository's whole scoped lifetime - see the comment on
    /// AddStaffManagementCoreClasses' AddDbContextFactory call for why a shared instance isn't safe
    /// in Blazor Server.</summary>
    public class BaseRepository
    {
        protected readonly IDbContextFactory<accountmanagementContext> _dbFactory;
        protected readonly IAuditLoggerService _log;
        protected readonly ICurrentUserContext _currentUser;
        protected readonly IMemoryCache _cache;
        protected CancellationTokenSource _resetCacheToken = new();

        public BaseRepository(IDbContextFactory<accountmanagementContext> dbFactory, IAuditLoggerService log, IMemoryCache memoryCache, ICurrentUserContext currentUser)
        {
            _dbFactory = dbFactory;
            _log = log;
            _cache = memoryCache;
            _currentUser = currentUser;
        }

        public void SetCache<T>(string name, T value) where T : notnull
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _cache.Set(name, value, options);
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

            FieldInfo field = typeof(MemoryCache).GetField("_stores", bindFlags)!;
            object[] cacheStores = (object[])field.GetValue(cache)!;

            List<MemoryCacheItemInfo> info = new();

            foreach (object cacheStore in cacheStores)
            {
                Type cacheStoreType = cacheStore.GetType();

                FieldInfo lockField = cacheStoreType.GetField("_entriesLock", bindFlags)!;
                object lockObj = lockField.GetValue(cacheStore)!;

                lock (lockObj)
                {
                    FieldInfo entriesField = cacheStoreType.GetField("_entries", bindFlags)!;
                    Hashtable entriesCollection = (Hashtable)entriesField.GetValue(cacheStore)!;

                    foreach (DictionaryEntry cacheItemEntry in entriesCollection)
                    {
                        Type cacheItemValueType = cacheItemEntry.Value!.GetType();

                        string key = (string)cacheItemEntry.Key.GetType().GetProperty("Key", bindFlags)!.GetValue(cacheItemEntry.Key)!;

                        PropertyInfo value = cacheItemValueType.GetProperty("Value", bindFlags)!;
                        PropertyInfo utcCreated = cacheItemValueType.GetProperty("UtcCreated", bindFlags)!;
                        PropertyInfo utcLastUpdateUsage = cacheItemValueType.GetProperty("UtcLastUpdateUsage", bindFlags)!;
                        PropertyInfo utcAbsoluteExpiry = cacheItemValueType.GetProperty("UtcAbsExp", bindFlags)!;
                        PropertyInfo utcSlidingExpiry = cacheItemValueType.GetProperty("SlidingExp", bindFlags)!;

                        var mcii = new MemoryCacheItemInfo(
                            key,
                            value.GetValue(cacheItemEntry.Value)!,
                            ((DateTime)utcCreated.GetValue(cacheItemEntry.Value)!).ToLocalTime(),
                            ((DateTime)utcLastUpdateUsage.GetValue(cacheItemEntry.Value)!).ToLocalTime(),
                            ((DateTime)utcAbsoluteExpiry.GetValue(cacheItemEntry.Value)!).ToLocalTime(),
                            (TimeSpan)utcSlidingExpiry.GetValue(cacheItemEntry.Value)!
                            );

                        info.Add(mcii);
                    }
                }
            }

            return info.ToArray();
        }
    }
}
