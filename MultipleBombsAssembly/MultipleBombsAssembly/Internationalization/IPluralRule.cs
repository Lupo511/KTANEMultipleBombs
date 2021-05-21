namespace MultipleBombsAssembly.Internationalization
{
    public interface IPluralRule
    {
        PluralQuantity GetQuantity(double n);
    }
}
