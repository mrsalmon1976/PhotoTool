using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.IO
{
    public interface IFileInfoWrapper
    {

        DateTime CreationTimeUtc { get; set; }
        string FullName { get; set; }

        string Extension { get; set; }

        long Length { get; set; }

        string Name { get; set; }

        string GetFileSizeReadable();
    }
    public class FileInfoWrapper : IFileInfoWrapper
    {
        private readonly FileInfo _fileInfo;

        public FileInfoWrapper(string path) : this(new FileInfo(path))
        {
        }

        public FileInfoWrapper(FileInfo fileInfo)
        {
            this._fileInfo = fileInfo;
            this.CreationTimeUtc = _fileInfo.CreationTimeUtc;
            this.Extension = _fileInfo.Extension;
            this.FullName = _fileInfo.FullName;
            this.Length = _fileInfo.Length;
            this.Name = _fileInfo.Name;
        }

        public DateTime CreationTimeUtc { get; set; }

        public string Extension { get; set; }

        public string FullName { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }


        public string GetFileSizeReadable()
        {
            long i = this.Length;

            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.##") + suffix;
        }

    }
}
