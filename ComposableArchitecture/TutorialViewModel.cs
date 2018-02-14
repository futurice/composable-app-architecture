using System;

namespace ComposableArchitecture
{
    internal class TutorialViewModel : ViewModel
    {
        private Func<bool> onCompleted;

        public TutorialViewModel(Func<bool> onCompleted)
        {
            this.onCompleted = onCompleted;
        }
    }
}