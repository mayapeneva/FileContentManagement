using FileContentManagement.Configuration;
using FileContentManagement.DTO;
using OneBitSoftware.Utilities;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileContentManagement
{
    public class FtpContentManager<TKey> : IContentManager<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly string url;
        private readonly NetworkCredential credentials;

        public FtpContentManager(FileManagementConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            url = $@"ftp://{configuration.Host}:{configuration.Port}/";
            credentials = new NetworkCredential(configuration.Username, configuration.Password);
        }

        public async Task<OperationResult> StoreAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var result = new OperationResult();
            if (fileContent.Stream is null)
            {
                result.AppendError(string.Format(MessageConstants.StoringFailed, id, "No file content to store."));
                return result;
            }

            try
            {
                var request = CreateRequest(id, WebRequestMethods.Ftp.UploadFile);
                using Stream requestStream = await request.GetRequestStreamAsync();
                await fileContent.Stream.CopyToAsync(requestStream, cancellationToken);

                using var response = (FtpWebResponse)await request.GetResponseAsync();
                if (response.StatusCode != FtpStatusCode.DataAlreadyOpen)
                {
                    result.AppendError(string.Format(MessageConstants.StoringFailed, id, response.StatusDescription));
                    return result;
                }

                result.AddSuccessMessage(string.Format(MessageConstants.StoringSuccess, id));
                return result;
            }
            catch (Exception ex)
            {
                result.AppendException(ex);
                return result;
            }
        }

        public async Task<OperationResult<bool>> ExistsAsync(TKey id, CancellationToken cancellationToken) 
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var request = CreateRequest(id, WebRequestMethods.Ftp.ListDirectory);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                if (response != null)
                {
                    response.Close();
                }

                return new OperationResult<bool>(true);
            }
            catch (Exception ex)
            {
                var result = new OperationResult<bool>(false);
                result.AppendException(ex);
                return result;
            }
        }

        public async Task<OperationResult<StreamInfo>> GetAsync(TKey id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var request = CreateRequest(id, WebRequestMethods.Ftp.DownloadFile);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var stream = response.GetResponseStream();
                var streamInfo = new StreamInfo
                {
                    Stream = stream
                };

                return new OperationResult<StreamInfo>(streamInfo);
            }
            catch (Exception ex)
            {
                var result = new OperationResult<StreamInfo>(new StreamInfo());
                result.AppendException(ex);
                return result;
            }
        }

        public async Task<OperationResult<byte[]>> GetBytesAsync(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var request = CreateRequest(id, WebRequestMethods.Ftp.DownloadFile);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var memoryStream = new MemoryStream();
                await response.GetResponseStream().CopyToAsync(memoryStream, cancellationToken);

                return new OperationResult<byte[]>(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                var result = new OperationResult<byte[]>(Array.Empty<byte>());
                result.AppendException(ex);
                return result;
            }
        }

        public async Task<OperationResult> UpdateAsync(TKey id, StreamInfo fileContent, CancellationToken cancellationToken)
        {
            var result = new OperationResult();
            if (fileContent.Stream is null)
            {
                result.AppendError(string.Format(MessageConstants.StoringFailed, id, "No file content to store."));
                return result;
            }

            var deleteResult = await DeleteAsync(id, cancellationToken);
            if (deleteResult.Fail)
            {
                return deleteResult;
            }

            return await StoreAsync(id, fileContent, cancellationToken);
        }

        public async Task<OperationResult> DeleteAsync(TKey id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new OperationResult();
            try
            {
                var request = CreateRequest(id, WebRequestMethods.Ftp.DeleteFile);
                var response = (FtpWebResponse)await request.GetResponseAsync();
                if (response.StatusCode != FtpStatusCode.FileActionOK)
                {
                    result.AppendError(string.Format(MessageConstants.DeletingFailed, id, response.StatusDescription));
                    return new OperationResult();
                }

                result.AddSuccessMessage(string.Format(MessageConstants.DeletingSuccess, id));
                return result;
            }
            catch (Exception ex)
            {
                result.AppendException(ex);
                return result;
            }
        }

        public async Task<OperationResult<string>> GetHashAsync(TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var request = CreateRequest(id, WebRequestMethods.Ftp.DownloadFile);
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var stream = response.GetResponseStream();

                var hash = await new MD5CryptoServiceProvider().ComputeHashAsync(stream, cancellationToken);
                return new OperationResult<string>(ByteArrayToString(hash));
            }
            catch (Exception ex)
            {
                var result = new OperationResult<string>(string.Empty);
                result.AppendException(ex);
                return result;
            }
        }

        private FtpWebRequest CreateRequest(TKey id, string method)
        {
            var uri = new Uri(url + id);
            var request = (FtpWebRequest)WebRequest.Create(uri);

            request.Method = method;
            request.Credentials = credentials;
            request.KeepAlive = true;

            return request;
        }

        private string ByteArrayToString(byte[] array)
        {
            var sb = new StringBuilder(array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                sb.Append(array[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
 