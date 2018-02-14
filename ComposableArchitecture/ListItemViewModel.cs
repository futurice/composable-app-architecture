namespace ComposableArchitecture
{
    internal class ContentBListItemViewModel : ViewModel
    {
        private ContentB b;

        public ContentBListItemViewModel(ContentB b)
        {
            this.b = b;
        }
    }

    internal class SimpleListItemViewModel : ViewModel
    {
        public SimpleListItemViewModel(string viewTemplate, string title, string description) : base(viewTemplate)
        {

        }
    }
}