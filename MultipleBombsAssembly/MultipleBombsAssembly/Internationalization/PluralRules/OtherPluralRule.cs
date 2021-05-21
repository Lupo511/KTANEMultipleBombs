namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class OtherPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            return PluralQuantity.Other;
        }
    }
}
