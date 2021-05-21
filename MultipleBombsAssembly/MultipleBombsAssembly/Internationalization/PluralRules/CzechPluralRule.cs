namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class CzechPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 1)
                return PluralQuantity.One;

            if (n.GetNumberOfDigitsAfterDecimal() != 0)
            {
                return PluralQuantity.Many;
            }

            if (n.IsBetween(2, 4))
                return PluralQuantity.Few;

            return PluralQuantity.Other;
        }
    }
}
