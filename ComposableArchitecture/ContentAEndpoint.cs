namespace ComposableArchitecture
{
    internal class ContentAEndpoint : Endpoint<ContentA>
    {

        public ContentAEndpoint(RelativeUri relativeUri) : base(relativeUri)
        {

        }

        public Response<ContentA> Invoke(string category)
        {

        }
    }
}