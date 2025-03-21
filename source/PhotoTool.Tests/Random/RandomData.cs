namespace PhotoTool.Tests.Random
{
    public static class RandomData
    {
        public static InternetRandomizer Internet { get; private set; } = new InternetRandomizer();

        public static NumberRandomizer Number { get; private set; } = new NumberRandomizer();

        public static StringRandomizer String { get; private set; } = new StringRandomizer();
    }
}
