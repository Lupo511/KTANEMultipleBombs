namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class SinhalaPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 0 || n == 1 || n.DigitsAfterDecimal() == 1)
                return PluralQuantity.One;
            return PluralQuantity.Other;
        }
    }
}
