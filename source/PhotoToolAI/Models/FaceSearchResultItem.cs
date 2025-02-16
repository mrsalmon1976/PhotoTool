using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Models
{
    public class FaceSearchResultItem
    {


        public string Name {  get; set; }

        public string Path { get; set; }

        public ImageSource Source { get; set; }

        public string MatchInfo { get; set; }

        public Color MatchColor { get; set; } = Colors.Black;

    }
}
