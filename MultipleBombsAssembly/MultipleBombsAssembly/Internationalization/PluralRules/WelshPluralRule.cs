namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class WelshPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            var isInt = n.IsInt();
            if (isInt)
            {
                var i = (int)n;
                switch (i)
                {
                    case 0:
                        return PluralQuantity.Zero;
                    case 1:
                        return PluralQuantity.One;
                    case 2:
                        return PluralQuantity.Two;
                    case 3:
                        return PluralQuantity.Few;
                    case 6:
                        return PluralQuantity.Many;
                }
            }
            return PluralQuantity.Other;
        }
    }
}
