using System.Threading.Tasks;

namespace ComposableArchitecture
{
    internal class NetworkDataSource<T> : DataSource<T>
    {

        private Endpoint<T> endpoint;

        public NetworkDataSource(Endpoint<T> endpoint, params Cache[] caches)
        {
            
        }


        internal async Task<bool> Reload()
        {

        }
    }

    internal class PagedNetworkDataSource<T> : NetworkDataSource<T>
    {
        public PagedNetworkDataSource(Endpoint<T> endpoint, params Cache[] caches) : base(endpoint, caches)
        {

        }

        public bool IsLoading { get; internal set; }

        public async Task<bool> LoadNextPage()
        {

        }
    }
}