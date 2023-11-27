using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;


namespace ProxySoap
{
    public class ProxyCache<T>
    {
        ObjectCache cache = MemoryCache.Default;
        DateTimeOffset dt_default = ObjectCache.InfiniteAbsoluteExpiration;

        // vérifie si présent dans le cache sinon fait appel au constructeur JCDecaux pour créer l'objet dans le cache
        public T Get(string cacheItem)
        {
            if(!cache.Contains(cacheItem)) {
                CacheItemPolicy policyTime=new CacheItemPolicy();
                policyTime.AbsoluteExpiration = dt_default;
                T objet=(T)Activator.CreateInstance(typeof(T), cacheItem);
                cache.Set(cacheItem, objet, policyTime);
            
            }
            return (T)cache.Get(cacheItem);
        }

        public T Get(string cacheItem, double dt_secs)
        {
            if(!cache.Contains(cacheItem))
            {
                CacheItemPolicy policyTime = new CacheItemPolicy();
                policyTime.AbsoluteExpiration=DateTimeOffset.Now.AddSeconds(dt_secs);
                T objet = (T)Activator.CreateInstance(typeof(T), cacheItem);
                cache.Set(cacheItem, objet, policyTime);
            }
            return (T)cache.Get(cacheItem);
        }

        public T Get(string cacheItem, DateTimeOffset dt)
        {
            if (!cache.Contains(cacheItem))
            {
                CacheItemPolicy policyTime= new CacheItemPolicy();
                policyTime.AbsoluteExpiration = dt;
                T objet=(T)Activator.CreateInstance(typeof(T),cacheItem);
                cache.Set(cacheItem, objet, policyTime);
            }
            return (T)cache.Get(cacheItem);
        }
    }
}
