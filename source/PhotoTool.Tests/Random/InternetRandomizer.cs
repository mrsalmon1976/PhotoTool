using System.Net;

namespace PhotoTool.Tests.Random
{
    public class InternetRandomizer
    {

        private static System.Random _random = new();

        public HttpMethod HttpMethod()
        {
            int i = _random.Next(1, 5);
            switch (i)
            {
                case 1:
                    return System.Net.Http.HttpMethod.Get;
                case 2:
                    return System.Net.Http.HttpMethod.Post;
                case 3:
                    return System.Net.Http.HttpMethod.Put;
                case 4:
                    return System.Net.Http.HttpMethod.Delete;
                case 5:
                    return System.Net.Http.HttpMethod.Patch;
            }

            throw new Exception("Unexpected number for switch method");
        }

        public HttpStatusCode HttpStatusCode()
        {
            var codes = Enum.GetValues(typeof(HttpStatusCode));
            int i = _random.Next(0, codes.Length - 1);
            return (HttpStatusCode)codes.GetValue(i)!;
        }


        public IPAddress IPAddress()
        {
            var data = new byte[4];
            _random.NextBytes(data);
            data[0] |= 1;
            return new IPAddress(data);
        }
    }
}
