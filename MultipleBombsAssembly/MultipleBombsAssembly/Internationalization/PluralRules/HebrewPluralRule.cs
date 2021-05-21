namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class HebrewPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                switch ((int)n)
                {
                    case 1:
                        return PluralQuantity.One;
                    case 2:
                        return PluralQuantity.Two;
                }

                if (n != 0 && (n % 10) == 0)
                {
                    return PluralQuantity.Many;
                }
            }
            return PluralQuantity.Other;

        }
    }
}
