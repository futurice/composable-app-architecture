using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ComposableArchitecture
{
    public class ExtensionsContainer : IEnumerable
    {
        private static ExtensionsContainer _instance;
        public static ExtensionsContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExtensionsContainer();
                }

                return _instance;
            }
        }

        private readonly List<object> _extensions = new List<object>();

        public void Add(params object[] extensions)
        {
            foreach (var ext in extensions)
            {
                _extensions.Add(ext);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _extensions.GetEnumerator();
        }

        public IEnumerable<TSlot> OfType<TSlot>()
        {
            return _extensions.OfType<TSlot>();
        }

        public void ForEach<TSlot>(Action<TSlot> action)
        {
            foreach (var ext in OfType<TSlot>())
            {
                action(ext);
            }
        }
    }
}
