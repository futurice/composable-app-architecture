namespace ComposableArchitecture
{
    internal class LoginService
    {
        public LoginService(LoginEndpoint endpoint)
        {
        }

        public readonly ReadOnlyObservableValue<bool> IsUserLoggedIn;

        public readonly ReadOnlyObservableValue<ContentType> UserAccess;


    }


    public enum ContentType
    {
        Free,
        Premium
    }
}