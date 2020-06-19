using Orleans;
using System.Threading.Tasks;

namespace Morstead.Orleans.Test.GrainInterfaces
{
    public interface IScalarValuePersistentGrain<T> : IGrainWithIntegerKey
    {
        Task SetScalarValue(T value);
        Task<T> GetScalarValue();
    }
}
