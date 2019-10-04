using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    public class Info
    {
        public string OdsCode { get; set; }

        public string NhsNumber { get; set; }

        public bool BetaTester { get; set; }
    }
}