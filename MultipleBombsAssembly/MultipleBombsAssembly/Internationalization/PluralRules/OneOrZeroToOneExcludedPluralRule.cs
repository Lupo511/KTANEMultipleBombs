namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class OneOrZeroToOneExcludedPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 0)
                return PluralQuantity.Zero;
            if ((int)n == 0 || (int)n == 1)
            {
                return PluralQuantity.One;
            }
            else
            {
                return PluralQuantity.Other;
            }
        }

    }
}
