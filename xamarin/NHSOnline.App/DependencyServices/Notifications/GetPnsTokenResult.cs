namespace NHSOnline.App.DependencyServices.Notifications
{
    public abstract class GetPnsTokenResult
    {
        private GetPnsTokenResult()
        {
        }

        public sealed class Authorised: GetPnsTokenResult
        {
            public GetPnsTokenResponse Response { get; }

            public Authorised(GetPnsTokenResponse response)
            {
                Response = response;
            }
        }

        public sealed class Unauthorised: GetPnsTokenResult
        {
        }
    }
}