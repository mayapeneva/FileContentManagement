using System.IO;

namespace FileContentManagement.DTO
{
    public class StreamInfo
    {
        public long Length { get; set; }

        public Stream Stream { get; set; }

        public ulong UnsignedLength => Length < 0 ? 0 : (ulong)Length;
    }
}
