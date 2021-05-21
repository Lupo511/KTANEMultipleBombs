namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class PolishPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if ((n % 10).IsBetween(2, 4) && !(n % 100).IsBetween(12, 14))
            {
                return PluralQuantity.Few;
            }
            if (n != 1 && (n % 10).IsBetween(0, 1) ||
                (n % 10).IsBetween(5, 9) ||
                (n % 100).IsBetween(12, 14))
            {
                return PluralQuantity.Many;
            }
            if (n == 1)
            {
                return PluralQuantity.One;
            }
            return PluralQuantity.Other;
        }
    }
}
