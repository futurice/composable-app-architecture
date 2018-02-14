namespace ComposableArchitecture
{
    internal class Endpoint<ReturnType> : DataSource<ReturnType>
    {
        private RelativeUri relativeUri;

        public Endpoint(RelativeUri relativeUri)
        {
            this.relativeUri = relativeUri;
        }
    }
}