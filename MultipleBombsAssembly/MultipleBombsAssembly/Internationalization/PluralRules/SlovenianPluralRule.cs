namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class SlovenianPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            var isInt = n.IsInt();
            if (isInt)
            {
                switch ((int)n)
                {
                    case 1:
                        return PluralQuantity.One;
                    case 2:
                        return PluralQuantity.Two;
                    case 3:
                    case 4:
                        return PluralQuantity.Two;
                }

                return PluralQuantity.Other;
            }
            else
            {
                return PluralQuantity.Few;
            }
        }
    }
}
