namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class SlavicPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                if ((n % 10) == 1 && (n % 100) != 11)
                {
                    return PluralQuantity.One;
                }
                if ((n % 10).IsBetween(2, 4) && !(n % 100).IsBetween(12, 14))
                {
                    return PluralQuantity.Few;
                }
                if ((n % 10) == 0 ||
                    (n % 10).IsBetween(5, 9) ||
                    (n % 100).IsBetween(11, 14))
                {
                    return PluralQuantity.Many;
                }

            }
            return PluralQuantity.Other;

        }
    }
}
