namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class ArabicPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                if (n == 0)
                {
                    return PluralQuantity.Zero;
                }
                if (n == 1)
                {
                    return PluralQuantity.One;
                }
                if (n == 2)
                {
                    return PluralQuantity.Two;
                }
                if ((n % 100).IsBetween(3, 10))
                {
                    return PluralQuantity.Few;
                }
                if ((n % 100).IsBetween(11, 99))
                {
                    return PluralQuantity.Many;
                }
            }
            return PluralQuantity.Other;

        }
    }
}
