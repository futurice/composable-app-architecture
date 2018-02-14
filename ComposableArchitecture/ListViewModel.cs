using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ComposableArchitecture
{
    internal class ListViewModel : ViewModel
    {
        public readonly ReadOnlyObservableValue<float> RelativePosition;

        public ListViewModel(string viewTemplate = null, params ViewModel[] items) : base(viewTemplate)
        {
        }

        public ListViewModel(string viewTemplate = null, Action<float> onRelativePositionChanged = null, IObservable<ViewModel> itemsSource = null) : base(viewTemplate)
        {
        }

    }

    internal class LazyListViewModel<T> : ViewModel
    {
        public LazyListViewModel(IReactiveCollection<T> itemsSource, Func<T, int, IEnumerable<ViewModel>> vmSelector, string viewTemplate = null) : base(viewTemplate)
        {

        }
    }
}