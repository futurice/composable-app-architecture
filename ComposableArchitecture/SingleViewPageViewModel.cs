namespace ComposableArchitecture
{
    internal class SingleViewPageViewModel : PageViewModel
    {

        public SingleViewPageViewModel(string id = null, ViewModel content = null, params UserCommandViewModel[] userCommands) : base(id)
        {
        }
    }
}