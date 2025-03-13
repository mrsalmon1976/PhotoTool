using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Shared.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<string> errors) : base()
        {
            this.Errors = new List<string>(errors);
        }

        public List<string> Errors { get; private set; }
    }
}
