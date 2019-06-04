using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Ndop {

    public class NdopConfigurationSettings {

        public string ClaimAudience { get; }

        public string ClaimIssuer { get; }

        public string CertificatePassword { get; }

        public string CertificatePath { get; }

        public NdopConfigurationSettings(string claimAudience, string claimIssuer, 
            string certificatePath, string certificatePassword) {

            ClaimAudience = claimAudience;
            ClaimIssuer = claimIssuer;
            CertificatePath = certificatePath;
            CertificatePassword = certificatePassword;

        }
    }
}