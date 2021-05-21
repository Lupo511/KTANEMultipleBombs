namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class IrishPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                if (n == 1)
                {
                    return PluralQuantity.One;
                }
                if (n == 2)
                {
                    return PluralQuantity.Two;
                }
                if ((n.IsBetween(3, 6)))
                {
                    return PluralQuantity.Few;
                }
                if ((n.IsBetween(7, 10)))
                {
                    return PluralQuantity.Many;
                }
            }
            return PluralQuantity.Other;

        }
    }
}
