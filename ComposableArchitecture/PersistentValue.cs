using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComposableArchitecture
{
    internal class PersistentValue<T> : ObservableValue<T>
    {
        public PersistentValue(string persistencyKey)
        {
        }

        internal async Task Clear()
        {
            throw new NotImplementedException();
        }
    }
}