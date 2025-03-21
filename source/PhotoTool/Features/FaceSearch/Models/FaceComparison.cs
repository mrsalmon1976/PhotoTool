using PhotoTool.Features.FaceSearch.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.FaceSearch.Models
{
    /// <summary>
    /// Represents the result of a comparison between two face regions detected in a larger image.
    /// </summary>
    public class FaceComparison
    {
        public float[]? Embedding { get; set; }

        public float DotProduct { get; set; }   

        public FaceMatchProspect FaceMatchProspect
        {
            get
            {
                if (DotProduct >= 0.42f)
                {
                    return FaceMatchProspect.Probable;
                }
                else if (DotProduct > 0.28f && DotProduct < 0.42f)
                {
                    return FaceMatchProspect.Possible;
                }
                return FaceMatchProspect.None;
            }
        }



        public static FaceComparison NoMatchFound
        {
            get
            {
                return new FaceComparison() { DotProduct = 0 }; 
            }
        }
    }
}
