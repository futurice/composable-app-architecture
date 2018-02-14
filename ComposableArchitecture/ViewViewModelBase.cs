using System;

namespace ComposableArchitecture
{
    internal class ViewViewModel : ViewModel
    {
        public ViewViewModel(string title, ViewModel content = null, IObservable<ViewModel> contentSource = null, string viewTemplate = null, params UserCommandViewModel[] userCommands) : base(viewTemplate)
        {
        }


    }
}