using System.Collections.Generic;
using System.IO;

namespace PhotoTool.Shared.Comparers
{
    internal class FileInfoCreateDateComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo? fileInfo1, FileInfo? fileInfo2)
        {
            if (fileInfo1 == null && fileInfo2 == null)
                return 0;
            else if (fileInfo2 == null)
                return -1;
            else if (fileInfo2 == null)
                return 1;

            return fileInfo1!.CreationTimeUtc.CompareTo(fileInfo2.CreationTimeUtc);
            
        }
    }
}
