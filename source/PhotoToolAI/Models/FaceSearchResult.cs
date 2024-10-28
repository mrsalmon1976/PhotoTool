using PhotoToolAI.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoToolAI.Models
{
    internal class FaceSearchResult
    {
        public float[]? Embedding { get; set; }

        public float DotProduct { get; set; }   

        public FaceMatchProspect FaceMatchProspect
        {
            get
            {
                if (DotProduct >= 0.42)
                {
                    return FaceMatchProspect.ProbableMatch;
                }
                else if (DotProduct > 0.28 && DotProduct < 0.42)
                {
                    return FaceMatchProspect.PossibleMatch;
                }
                return FaceMatchProspect.NoMatch;
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
