using System;
using System.Threading.Tasks;

namespace ComposableArchitecture
{
    internal class UserCommandViewModel : ViewModel
    {
        public UserCommandViewModel(string label, string viewTemplate = null, Icon icon = Icon.None, params Func<Task>[] actions) : base(viewTemplate)
        {
        }
    }

    public enum Icon
    {
        None,
        Refresh,
        Clear
    }
}