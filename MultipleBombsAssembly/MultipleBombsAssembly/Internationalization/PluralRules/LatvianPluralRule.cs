namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class LatvianPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 0 || (n % 100).IsBetween(11, 19))
            {
                return PluralQuantity.Zero;
            }

            var f = n.DigitsAfterDecimal();
            if (f.IsBetween(11, 19))
                return PluralQuantity.Zero;

            if (n % 10 == 1 && n % 100 != 11)
                return PluralQuantity.One;
            if (f % 10 == 1)
            {
                if (n.GetNumberOfDigitsAfterDecimal() == 2)
                {
                    if (f % 100 != 11)
                        return PluralQuantity.One;
                }
                else
                {
                    return PluralQuantity.One;
                }
            }
            return PluralQuantity.Other;
        }
    }
}
