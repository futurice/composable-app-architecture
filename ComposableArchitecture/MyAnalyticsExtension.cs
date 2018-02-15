using System.Collections.Generic;

namespace ComposableArchitecture
{
    internal class MyAnalyticsExtension : Dictionary<string, string>, IViewModelVisibilityChangedListener
    {
        private ProdDevUri endpoint;

        public MyAnalyticsExtension(ProdDevUri endpoint)
        {
            this.endpoint = endpoint;
        }

        void IViewModelVisibilityChangedListener.OnChanged(ViewModel viewModel, ViewModelVisibility visiblity)
        {
            if (visiblity != ViewModelVisibility.Shown
                || !TryGetValue(viewModel.Id, out string key))
            {
                return;
            }

            if (viewModel is PageViewModel page) {
                SendView("page/" + key);
            }
        }

        public void SendView(string name)
        {

        }
    }

    public interface IViewModelVisibilityChangedListener
    {
        void OnChanged(ViewModel viewModel, ViewModelVisibility visiblity);
    }

    public enum ViewModelVisibility
    {
        Hidden,
        Showing,
        Shown,
        Hiding
    }

}