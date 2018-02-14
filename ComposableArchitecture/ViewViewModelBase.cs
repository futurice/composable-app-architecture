namespace ComposableArchitecture
{
    internal class ViewViewModel : ViewModel
    {
        public ViewViewModel(string title, ViewModel content, string viewTemplate = null, params UserCommandViewModel[] userCommands) : base(viewTemplate)
        {
        }
    }
}