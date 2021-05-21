namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class CroatPluralRule : IPluralRule
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
                if ((integer % 10).IsBetween(2, 4) && !(integer % 100).IsBetween(12, 14))
                {
                    return PluralQuantity.Few;
                }
            }
            var f = n.DigitsAfterDecimal();
            if (f % 10 == 1 && f % 100 != 11)
            {
                return PluralQuantity.One;
            }

            if ((f % 10).IsBetween(2, 4) && !(f % 100).IsBetween(12, 14))
            {
                return PluralQuantity.Few;
            }

            return PluralQuantity.Other;
        }
    }
}
