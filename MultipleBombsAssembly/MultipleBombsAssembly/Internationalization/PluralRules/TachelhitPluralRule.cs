namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class TachelhitPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if ((int)n == 0 || n == 1)
            {
                return PluralQuantity.One;
            }
            if (n <= 10 && (int)n == n)
                return PluralQuantity.Few;

            return PluralQuantity.Other;
        }
    }
}
