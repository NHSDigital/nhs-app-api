using System;

namespace NHSOnline.Backend.Auth.APIM
{
    public interface IApimJwtHelper
    {
        string CreateApimJwt(
            Uri audience,
            string certPath,
            string certPassphrase,
            string key,
            string kid);
    }
}