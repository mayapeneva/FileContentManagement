namespace FileContentManagement
{
    using FileContentManagement.DTO;
    using OneBitSoftware.Utilities;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class ContentManager<TKey> : IContentManager<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public Task<OperationResult> DeleteAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<StreamInfo>> GetAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<byte[]>> GetBytesAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<string>> GetHashAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> StoreAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdateAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
