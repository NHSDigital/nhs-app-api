using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.Support
{
    [SuppressMessage("Microsoft.Naming", "CA1717", Justification = "False positive - Api is not a plural word")]
    public enum SourceApi
    {
        None = 0,
        Emis = 1,
        Tpp = 2,
        Vision = 3,
        Microtest = 4,
        NhsLogin = 5,
        ServiceJourneyRules = 6,
        OrganDonation = 7,
        UserInfo = 8,
        OnlineConsultations = 9,
        Qualtrics = 10
    }
}