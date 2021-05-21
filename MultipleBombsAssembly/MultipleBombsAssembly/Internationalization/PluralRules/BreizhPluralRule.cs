namespace MultipleBombsAssembly.Internationalization.PluralRules
{
    internal class BreizhPluralRule : IPluralRule
    {
        public PluralQuantity GetQuantity(double n)
        {
            if (n.IsInt())
            {
                var mod10 = n % 10;
                var mod100 = n % 100;
                if (mod10 == 1 && mod100 != 11 && mod100 != 71 && mod100 != 91)
                {
                    return PluralQuantity.One;
                }
                if (mod10 == 2 && mod100 != 12 && mod100 != 72 && mod100 != 92)
                {
                    return PluralQuantity.Two;
                }
                var diffMod = mod100 - mod10;
                if ((mod10 == 3 || mod10 == 4 || mod10 == 9) && diffMod != 10 && diffMod != 70 && diffMod != 90)
                {
                    return PluralQuantity.Few;
                }
                if (n != 0 && n % 1000000 == 0)
                    return PluralQuantity.Many;
            }
            return PluralQuantity.Other;
        }
    }
}
