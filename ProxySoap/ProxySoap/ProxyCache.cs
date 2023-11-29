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
        // Cache utilisé pour stocker les objets en mémoire
        ObjectCache cache = MemoryCache.Default;

        // Durée d'expiration par défaut (infinie)
        DateTimeOffset dt_default = ObjectCache.InfiniteAbsoluteExpiration;

        // Vérifie si l'objet est présent dans le cache, sinon fait appel au constructeur JCDecaux pour créer l'objet dans le cache
        public T Get(string cacheItem)
        {
            if (!cache.Contains(cacheItem))
            {
                // Politique d'expiration par défaut
                CacheItemPolicy policyTime = new CacheItemPolicy();
                policyTime.AbsoluteExpiration = dt_default;

                // Création de l'objet en utilisant le constructeur JCDecaux
                T objet = (T)Activator.CreateInstance(typeof(T), cacheItem);
                cache.Set(cacheItem, objet, policyTime);
            }
            return (T)cache.Get(cacheItem);
        }

        // Même que l'autre mais ici on spécifie le temps d'existence de l'obj en secondes
        public T Get(string cacheItem, double dt_secs)
        {
            if (!cache.Contains(cacheItem))
            {
                // Spécification du temps en secondes
                CacheItemPolicy policyTime = new CacheItemPolicy();
                policyTime.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(dt_secs);

                // Création de l'objet en utilisant le constructeur JCDecaux
                T objet = (T)Activator.CreateInstance(typeof(T), cacheItem);
                cache.Set(cacheItem, objet, policyTime);
            }
            return (T)cache.Get(cacheItem);
        }

        // Pareil mais on peut définir quand expire l'obj
        public T Get(string cacheItem, DateTimeOffset dt)
        {
            if (!cache.Contains(cacheItem))
            {
                
                CacheItemPolicy policyTime = new CacheItemPolicy();
                policyTime.AbsoluteExpiration = dt;

                // Création de l'objet en utilisant le constructeur JCDecaux
                T objet = (T)Activator.CreateInstance(typeof(T), cacheItem);
                cache.Set(cacheItem, objet, policyTime);
            }
            return (T)cache.Get(cacheItem);
        }
    }
}
