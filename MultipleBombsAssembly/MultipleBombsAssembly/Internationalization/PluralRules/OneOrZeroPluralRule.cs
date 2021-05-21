namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class OneOrZeroPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 0)
                return PluralQuantity.Zero;
            if (n == 1)
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
