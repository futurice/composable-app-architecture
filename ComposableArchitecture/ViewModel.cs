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
            ExtensionsContainer
                .Instance
                .ForEach<IViewModelVisibilityChangedListener>(ext => ext.OnChanged(this, visiblity));
        }
    }
}