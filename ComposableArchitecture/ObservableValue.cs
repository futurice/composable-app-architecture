using System;

namespace ComposableArchitecture
{
    public class ObservableValue<T> : ReadOnlyObservableValue<T>, ISubject<T>
    {
        public new T Value {
            get { return base.Value; }
            set { base.Value = value; }
        }
    }

    public class ReadOnlyObservableValue<T> : IObservable<T>
    {
        public ReadOnlyObservableValue(ViewModel self)
        {
        }

        public T Value { get; protected set; }
        
    }
}