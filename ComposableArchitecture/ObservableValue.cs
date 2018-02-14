using System;

namespace ComposableArchitecture
{
    public class ObservableValue<T> : ReadOnlyObservableValue<T>, ISubject<T>
    {

    }

    public class ReadOnlyObservableValue<T> : IObservable<T>
    {

    }
}