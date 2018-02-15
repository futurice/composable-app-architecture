using System;
using System.Collections.Generic;

namespace ComposableArchitecture
{
    internal class ViewViewModel : ViewModel
    {
        public ViewViewModel(
            string title, 
            ViewModel content = null, 
            IObservable<ViewModel> contentSource = null, 
            string id = null,
            string viewTemplate = null, 
            IObservable<IEnumerable<UserCommandViewModel>> userCommandsSource = null, 
            params UserCommandViewModel[] userCommands
        ) : base(id, viewTemplate)
        {

        }


    }
}