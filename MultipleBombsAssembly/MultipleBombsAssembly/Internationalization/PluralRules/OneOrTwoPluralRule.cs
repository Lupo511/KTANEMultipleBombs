namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class OneOrTwoPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 1)
            {
                return PluralQuantity.One;
            }
            else if (n == 2)
            {
                return PluralQuantity.Two;
            }
            else
            {
                return PluralQuantity.Other;
            }
        }

    }
}
