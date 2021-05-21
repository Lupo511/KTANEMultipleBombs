namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class ScottishGaelicPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                var i = (int)n;
                switch (i)
                {
                    case 1:
                    case 11:
                        return PluralQuantity.One;
                    case 2:
                    case 12:
                        return PluralQuantity.Two;
                }
                if (i <= 19)
                    return PluralQuantity.Few;
            }
            return PluralQuantity.Other;
        }
    }
}
