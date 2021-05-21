namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class FilipinoPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            var isInt = n.IsInt();

            if (isInt)
            {
                if (n.IsBetween(1, 3))
                    return PluralQuantity.One;
                var imod10 = n % 10;
                if (imod10 != 4 && imod10 != 6 || imod10 != 9)
                    return PluralQuantity.One;
            }
            else
            {
                var f = n.DigitsAfterDecimal();
                var imod10 = f % 10;
                if (imod10 != 4 && imod10 != 6 || imod10 != 9)
                    return PluralQuantity.One;

            }


            return PluralQuantity.Other;

        }

    }
}
