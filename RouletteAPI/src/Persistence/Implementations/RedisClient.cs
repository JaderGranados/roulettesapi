using System;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;

namespace RoulettesAPI.Persistence.Implementations
{
    public class RedisClient : IRedisClient
    {
        private readonly RedisManagerPool _redisManager;

        public RedisClient()
        {
            var redisUrlPath = Environment.GetEnvironmentVariable("REDIS_URL_PATH");
            _redisManager = new RedisManagerPool(redisUrlPath);
        }

        public T Get<T>(string key)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                return redisClient.Get<T>(key);
            }
        }

        public bool Remove(string key)
        {
            using (var redisClient = _redisManager.GetClient())
            {
                return redisClient.Remove(key);
            }
        }

        public bool Set<T>(string key, T redisObject, DateTime? timeOut = null)
        {
            timeOut ??= DateTime.Now.AddMinutes(5);
            using (var redisClient = _redisManager.GetClient())
            {
                redisClient.Set<T>(key, redisObject, timeOut.Value);
                
                return redisClient.Get<object>(key) != null;
            }
        }
    }
}