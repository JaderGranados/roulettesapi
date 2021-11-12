using System;
using System.Threading.Tasks;

namespace RoulettesAPI.Persistence
{
    public interface IRedisClient
    {
        bool Set<T>(string key, T redisObject, DateTime? timeOut = null);

        T Get<T>(string key);

        bool Remove(string key);
    }
}