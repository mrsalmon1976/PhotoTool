using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.IO
{
    public interface IDirectoryInfoWrapper
    {

        DateTime CreationTimeUtc { get; set; }

        string FullName { get; set; }

        string Name { get; set; }
    }
    public class DirectoryInfoWrapper : IDirectoryInfoWrapper
    {
        private readonly DirectoryInfo _directoryInfo;

        public DirectoryInfoWrapper(string path) : this(new DirectoryInfo(path))
        {
        }

        public DirectoryInfoWrapper(DirectoryInfo directoryInfo)
        {
            this._directoryInfo = directoryInfo;
            this.CreationTimeUtc = _directoryInfo.CreationTimeUtc;
            this.FullName = _directoryInfo.FullName;
            this.Name = _directoryInfo.Name;
        }

        public DateTime CreationTimeUtc { get; set; }

        public string FullName { get; set; }


        public string Name { get; set; }

    }
}
