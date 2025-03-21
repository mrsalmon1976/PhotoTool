using System.Text;

namespace PhotoTool.Tests.Random
{
    public class StringRandomizer
    {
        private static System.Random _random = new System.Random();

        private const string AlphaCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string Letters()
        {
            return Letters(0, int.MaxValue);
        }

        public string Letters(int maxLength)
        {
            return Letters(0, maxLength);
        }

        public string Letters(int minLength, int maxLength)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetRandomLetters(minLength, 100));
            if (sb.Length > maxLength)
            {
                sb.Remove(maxLength, sb.Length - maxLength);
            }
            return sb.ToString();
        }

        private string GetRandomLetters(int min, int max)
        {
            int length = _random.Next(min, max);
            return new string(Enumerable.Repeat(AlphaCharacters, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
