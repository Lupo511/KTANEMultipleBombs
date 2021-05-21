namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class MacedonianPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                if (n % 10 == 1)
                {
                    return PluralQuantity.One;
                }
            }
            else
            {
                var f = n.DigitsAfterDecimal();
                if (f % 10 == 1)
                {
                    return PluralQuantity.One;
                }
            }
            return PluralQuantity.Other;
        }
    }
}
