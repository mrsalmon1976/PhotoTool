using PhotoTool.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTool.Tests
{
    public class TestDataUtils
    {
        public static IEnumerable<T> CreateMany<T>(int count) where T : class 
        {
            List<T> results = new List<T>();
            for (int i = 0; i < count; i++)
            {
                results.Add(new SubstituteBuilder<T>().WithRandomProperties().Build());
            }
            return results;
        }
    }
}
