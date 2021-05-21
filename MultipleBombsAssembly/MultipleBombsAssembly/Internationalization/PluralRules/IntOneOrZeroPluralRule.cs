namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class IntOneOrZeroPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 0 || n == 1)
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
