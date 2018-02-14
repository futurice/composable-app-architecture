namespace ComposableArchitecture
{
    internal class SingleViewPageViewModel : PageViewModel
    {
        private ListViewModel reloadableListViewModel;

        public SingleViewPageViewModel(ListViewModel reloadableListViewModel, params UserCommandViewModel[] userCommands)
        {
            this.reloadableListViewModel = reloadableListViewModel;
        }
    }
}