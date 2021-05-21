namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class RomanianPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.GetNumberOfDigitsAfterDecimal() > 0 || n == 0 || (n != 1 && (n % 100).IsBetween(1, 19)))
            {
                return PluralQuantity.Few;
            }
            if (n == 1)
            {
                return PluralQuantity.One;
            }
            return PluralQuantity.Other;

        }
    }
}
