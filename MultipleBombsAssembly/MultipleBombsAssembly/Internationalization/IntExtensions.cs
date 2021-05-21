namespace MultipleBombsAssembly.Internationalization
{
    internal static class IntExtensions
    {
        public static bool IsBetween(this int number, int start, int end)
        {
            return start <= number && number <= end;
        }
        public static bool IsBetweenEndNotIncluded(this int number, int start, int end)
        {
            return start <= number && number < end;
        }

        public static bool IsBetween(this long number, long start, long end)
        {
            return start <= number && number <= end;
        }
        public static bool IsBetweenEndNotIncluded(this long number, long start, long end)
        {
            return start <= number && number < end;
        }


    }
}
