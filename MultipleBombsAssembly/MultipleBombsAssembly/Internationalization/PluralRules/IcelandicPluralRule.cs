namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class IcelandicPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                var integer = (int)n;
                if (integer % 10 == 1 && integer % 100 != 11)
                {
                    return PluralQuantity.One;
                }
                return PluralQuantity.Other;
            }
            else
            {
                return PluralQuantity.One;
            }


        }
    }
}
