using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Models
{
    public class FileLoadInfo
    {
        public string[] Files { get; set; }

        public bool ReportErrors { get; set; }

        public int ErrorCount { get; set; }

    }
}
