using FileContentManagement.DTO;
using OneBitSoftware.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FileContentManagement
{
    public interface IContentManager<in TKey>
        where TKey : struct, IEquatable<TKey>
    {
        Task<OperationResult> StoreAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken);

        Task<OperationResult<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken);

        Task<OperationResult<StreamInfo>> GetAsync(TKey id, CancellationToken cancellationToken);

        Task<OperationResult<byte[]>> GetBytesAsync(TKey id, CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(TKey id, CancellationToken cancellationToken);

        Task<OperationResult<string>> GetHashAsync(TKey id, CancellationToken cancellationToken);
    }
}
