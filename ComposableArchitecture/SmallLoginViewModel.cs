namespace ComposableArchitecture
{
    internal class LoginViewModel : ViewModel
    {
        public LoginViewModel(LoginService loginService, string viewTemplate = null) : base(viewTemplate)
        {
        }
    }
}