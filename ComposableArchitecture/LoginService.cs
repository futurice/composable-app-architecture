namespace ComposableArchitecture
{
    internal class LoginService
    {
        public LoginService(LoginEndpoint endpoint)
        {
        }

        public readonly ReadOnlyObservableValue<bool> IsUserLoggedIn;
    }
}