using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ComposableArchitecture
{
    internal class DataSource<T>
    {
        public IObservable<T> Items { get; internal set; }

    }
}