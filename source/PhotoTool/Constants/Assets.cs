using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Constants
{
    public class Assets
    {
        private Assets(string value) { Uri = value; }

        public string Uri { get; private set; }

        public static Assets PhotoToolLogo_300x300_800bg { get { return new Assets("avares://PhotoTool/Assets/phototool-logo_300x300_800bg.png"); } }

        public override string ToString()
        {
            return Uri;

        }
    }
}
