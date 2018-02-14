namespace ComposableArchitecture
{
    internal class SuperViewModel : ViewModel
    {
        private SuperContentB b;

        public SuperViewModel(SuperContentB b, string viewTemplate = null) : base(viewTemplate)
        {
            this.b = b;
        }
    }
}