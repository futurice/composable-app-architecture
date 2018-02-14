namespace ComposableArchitecture
{
    internal class ProdDevUri
    {
        public ProdDevUri(System.Uri prod, System.Uri dev)
        {
        }

        public ISubject<bool> UseDevelopment { get; internal set; }
    }
}