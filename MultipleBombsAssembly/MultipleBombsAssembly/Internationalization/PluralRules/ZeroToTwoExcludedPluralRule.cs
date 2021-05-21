namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class ZeroToTwoExcludedPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsBetweenEndNotIncluded(0, 2))
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
