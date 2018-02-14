using System;

namespace ComposableArchitecture
{
    internal class RelativeUri
    {
        private ProdDevUri contentUri;
        private Uri uri;

        public RelativeUri(ProdDevUri contentUri, Uri uri)
        {
            this.contentUri = contentUri;
            this.uri = uri;
        }
    }
}