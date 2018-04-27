namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class UserInfoResponse
    {
        public string Ods_code { get; }
        public string Im1_connection_token { get; }

        public UserInfoResponse(string odsCode, string im1ConnectionToken)
        {
            Ods_code = odsCode;
            Im1_connection_token = im1ConnectionToken;
        }
    }
}
