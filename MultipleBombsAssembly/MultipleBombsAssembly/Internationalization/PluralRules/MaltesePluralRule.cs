namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class MaltesePluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            var isInt = n.IsInt();
            if (isInt)
            {
                var i = (int)n;
                if (i == 1)
                {
                    return PluralQuantity.One;
                }
                if (i == 0 || (i % 100).IsBetween(2, 10))
                {
                    return PluralQuantity.Few;
                }
                if ((i % 100).IsBetween(11, 19))
                {
                    return PluralQuantity.Many;
                }
            }

            return PluralQuantity.Other;

        }
    }
}
