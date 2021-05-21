namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class OnlyOnePluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 1)
            {
                return PluralQuantity.One;
            }
            else
            {
                return PluralQuantity.Other;
            }
        }

    }
}
