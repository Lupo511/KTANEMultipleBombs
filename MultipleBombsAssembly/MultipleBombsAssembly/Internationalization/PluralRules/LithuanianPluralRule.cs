namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class LithuanianPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if ((n % 10).IsBetween(2, 9) && !(n % 100).IsBetween(11, 19))
            {
                return PluralQuantity.Few;
            }
            if ((n % 10) == 1 && !(n % 100).IsBetween(11, 19))
            {
                return PluralQuantity.One;
            }
            if (n.GetNumberOfDigitsAfterDecimal() != 0)
                return PluralQuantity.Many;
            return PluralQuantity.Other;

        }
    }
}
