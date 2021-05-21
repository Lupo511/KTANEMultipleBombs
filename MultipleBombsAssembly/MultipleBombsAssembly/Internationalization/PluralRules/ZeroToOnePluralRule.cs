namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class ZeroToOnePluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsBetween(0, 1))
            {
                return PluralQuantity.One;
            }
            return PluralQuantity.Other;

        }
    }
}
