namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class DanishPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n != 0 && n.IsBetween(0, 1))
                return PluralQuantity.One;
            return PluralQuantity.Other;
        }
    }
}
