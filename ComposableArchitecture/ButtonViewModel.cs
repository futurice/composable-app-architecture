using System;
using System.Threading.Tasks;

namespace ComposableArchitecture
{
    internal class UserCommandViewModel : ViewModel
    {
        public UserCommandViewModel(string label, string viewTemplate = null, params Func<Task>[] actions) : base(viewTemplate)
        {
        }
    }
}