using PhotoTool.Features.FaceSearch.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Features.FaceSearch.Models
{
    public class FaceSearchResult
    {
        public float[]? Embedding { get; set; }

        public float DotProduct { get; set; }   

        public FaceMatchProspect FaceMatchProspect
        {
            get
            {
                if (DotProduct >= 0.42)
                {
                    return FaceMatchProspect.Probable;
                }
                else if (DotProduct > 0.28 && DotProduct < 0.42)
                {
                    return FaceMatchProspect.Possible;
                }
                return FaceMatchProspect.None;
            }
        }



        public static FaceSearchResult NoMatchFound
        {
            get
            {
                return new FaceSearchResult() { DotProduct = 0 }; 
            }
        }
    }
}
