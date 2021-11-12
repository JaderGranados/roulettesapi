using System.Threading.Tasks;

namespace RoulettesAPI.Persistence
{
    public interface IDbContext
    {
        Task<T> ExecuteFunction<T> (string inputObjectStringify, string functionName = null);

        IDbContext WithFunctionName(string functionName);
    }
}