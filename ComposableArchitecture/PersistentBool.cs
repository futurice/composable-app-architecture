using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComposableArchitecture
{
    internal class PersistentBool : ISubject<bool>
    {
        public PersistentBool(string v)
        {
        }

        internal async Task Clear()
        {
            throw new NotImplementedException();
        }
    }
}