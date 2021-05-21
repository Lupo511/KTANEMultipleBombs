namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class ManxPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            var isInt = n.IsInt();
            var i = (int)n;
            if (isInt)
            {
                if (i % 10 == 1)
                    return PluralQuantity.One;
                if (i % 10 == 2)
                    return PluralQuantity.Two;
                if (i % 20 == 0)
                    return PluralQuantity.Few;
                return PluralQuantity.Other;
            }
            else
            {
                return PluralQuantity.Many;
            }
        }
    }
}
