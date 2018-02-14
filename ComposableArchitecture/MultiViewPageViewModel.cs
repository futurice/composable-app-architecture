namespace ComposableArchitecture
{
    internal class MultiViewPageViewModel : PageViewModel
    {
        private ViewViewModel[] listViewModel;

        public MultiViewPageViewModel(params ViewViewModel[] listViewModel)
        {
            this.listViewModel = listViewModel;
        }
    }
}