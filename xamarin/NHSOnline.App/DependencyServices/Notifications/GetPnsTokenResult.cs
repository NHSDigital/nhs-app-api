namespace NHSOnline.App.DependencyServices.Notifications
{
    public abstract class GetPnsTokenResult
    {
        private GetPnsTokenResult()
        {
        }

        public sealed class Authorised: GetPnsTokenResult
        {
            public Authorised(GetPnsTokenResponse response)
            {
                Response = response;
            }

            private GetPnsTokenResponse Response { get; }
            public string DevicePns => Response.DevicePns;
            public string DeviceType => Response.DeviceType;
        }

        public sealed class Unauthorised: GetPnsTokenResult
        {
        }
    }
}