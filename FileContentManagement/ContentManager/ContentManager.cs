using FileContentManagement.Configuration;
using FileContentManagement.DTO;
using OneBitSoftware.Utilities;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FileContentManagement
{
    internal class ContentManager<TKey> : IContentManager<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly FileManagementConfiguration configuration;

        public ContentManager(FileManagementConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.configuration = configuration;
        }

        public async Task<OperationResult> StoreAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var result = new OperationResult();
            if (fileContent.Stream is null)
            {
                result.AppendError(string.Format(MessageConstants.StoringFail, id, "No file content to store."));
                return result;
            }

            try
            {
                var uri = new Uri($@"ftp://{configuration.Host}:{configuration.Port}/{id}");
                var request = (FtpWebRequest)WebRequest.Create(uri);

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(configuration.Username, configuration.Password);                
                request.ContentLength = fileContent.Stream.Length;

                using Stream requestStream = request.GetRequestStream();
                await fileContent.Stream.CopyToAsync(requestStream, cancellationToken);

                using var response = (FtpWebResponse)request.GetResponse();
                if (response.StatusCode != FtpStatusCode.FileActionOK)
                {
                    result.AppendError(string.Format(MessageConstants.StoringFail, id, response.StatusDescription));
                    return result;
                }

                result.AddSuccessMessage(string.Format(MessageConstants.StoringSuccess, id));
                return result;
            }
            catch (Exception ex)
            {
                result.AppendError(string.Format(MessageConstants.StoringFail, id, ex.Message));
                return result;
            }
        }

        public async Task<OperationResult<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var uri = new Uri($@"ftp://{configuration.Host}:{configuration.Port}/{id}");
                var request = (FtpWebRequest)WebRequest.Create(uri);

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(configuration.Username, configuration.Password);

                using var response = (FtpWebResponse)request.GetResponse();
                if (response != null)
                {
                    response.Close();
                }

                return new OperationResult<bool>(true);
            }
            catch (Exception ex)
            {
                var result = new OperationResult<bool>(false);
                result.AppendError(string.Format(MessageConstants.FileDoesNotExist, id, ex.Message));
                return result;
            }
        }

        public Task<OperationResult<StreamInfo>> GetAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<byte[]>> GetBytesAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> UpdateAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> DeleteAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<string>> GetHashAsync(TKey id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
