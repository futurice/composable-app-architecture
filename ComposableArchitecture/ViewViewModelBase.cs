using System;
using System.Collections.Generic;

namespace ComposableArchitecture
{
    internal class ViewViewModel : ViewModel
    {
        private readonly IObservable<ViewModel> _contentSource;
        private readonly IObservable<IEnumerable<UserCommandViewModel>> _userCommandSource;
        private IDisposable _contentSubscription;

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
            _contentSource = contentSource;
            _userCommandSource = userCommandsSource;
        }

        protected override void OnActivating()
        {
            base.OnActivating();

            _contentSubscription = _contentSource?.Subscribe(vm => {

            });
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            _contentSubscription?.Dispose();
            _contentSubscription = null;
        }
    }
}