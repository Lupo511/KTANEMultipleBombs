namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class CentralAtlasTamazightPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n == 0 || n == 1 || (n.IsInt() && n.IsBetween(11, 99)))
            {
                return PluralQuantity.One;
            }
            return PluralQuantity.Other;
        }
    }
}
