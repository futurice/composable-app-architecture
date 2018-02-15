using System;
using System.Reactive.Linq;

namespace ComposableArchitecture
{
    public class ViewModel
    {
        public readonly string Id;

        public ViewModel(string id = null, string viewTemplate = null)
        {
            Id = id;
        }
        
        public void NotifyVisibilityChanged(ViewModelVisibility visiblity)
        {
            if (visiblity == ViewModelVisibility.Showing)
            {
                OnActivating();
            }
            else if (visiblity == ViewModelVisibility.Hidden)
            {
                OnDeactivated();
            }

            ExtensionsContainer
                .Instance
                .ForEach<IViewModelVisibilityChangedListener>(ext => ext.OnChanged(this, visiblity));
        }

        protected virtual void OnDeactivated()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnActivating()
        {

        }
    }
}