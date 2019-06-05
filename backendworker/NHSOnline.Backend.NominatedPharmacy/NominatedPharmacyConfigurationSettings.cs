using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyConfigurationSettings : INominatedPharmacyConfigurationSettings
    {
        public NominatedPharmacyConfigurationSettings(
            bool isNominatedPharmacyEnabled,
            Uri baseUrl,
            string spineAccreditedSystemIdFrom,
            string spineAccreditedSystemIdTo,
            string pdsQueryFromAddress,
            string pdsQueryToAddress,
            string partSdsRoleId,
            string sdsUserId,
            string personSdsRoleId,
            int artificialDelayAfterNominatedPharmacyUpdateInMilliseconds)
        {
            IsNominatedPharmacyEnabled = isNominatedPharmacyEnabled;
            BaseUrl = baseUrl;
            SpineAccreditedSystemIdFrom = spineAccreditedSystemIdFrom;
            SpineAccreditedSystemIdTo = spineAccreditedSystemIdTo;
            PdsQueryFromAddress = pdsQueryFromAddress;
            PdsQueryTo = pdsQueryToAddress;
            PartSdsRoleId = partSdsRoleId;
            SdsUserId = sdsUserId;
            PersonSdsRoleId = personSdsRoleId;
            ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds = artificialDelayAfterNominatedPharmacyUpdateInMilliseconds;
        }

        public bool IsNominatedPharmacyEnabled { get; }

        public Uri BaseUrl { get; }

        public string SpineAccreditedSystemIdFrom { get; }

        public string SpineAccreditedSystemIdTo { get; }

        public string PdsQueryFromAddress { get; }

        public string PdsQueryTo { get; }

        public string PartSdsRoleId { get; }

        public string SdsUserId { get; }

        public string PersonSdsRoleId { get; }

        public int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }

        public void Validate()
        {
            if (BaseUrl == null)
            {
                throw new ConfigurationNotFoundException(nameof(BaseUrl));
            }

            if (string.IsNullOrEmpty(SpineAccreditedSystemIdFrom))
            {
                throw new ConfigurationNotFoundException(nameof(SpineAccreditedSystemIdFrom));
            }

            if (string.IsNullOrEmpty(SpineAccreditedSystemIdTo))
            {
                throw new ConfigurationNotFoundException(nameof(SpineAccreditedSystemIdTo));
            }

            if (string.IsNullOrEmpty(PdsQueryFromAddress))
            {
                throw new ConfigurationNotFoundException(nameof(PdsQueryFromAddress));
            }

            if (string.IsNullOrEmpty(PdsQueryTo))
            {
                throw new ConfigurationNotFoundException(nameof(PdsQueryTo));
            }

            if (string.IsNullOrEmpty(PartSdsRoleId))
            {
                throw new ConfigurationNotFoundException(nameof(PartSdsRoleId));
            }

            if (string.IsNullOrEmpty(SdsUserId))
            {
                throw new ConfigurationNotFoundException(nameof(SdsUserId));
            }

            if (string.IsNullOrEmpty(PersonSdsRoleId))
            {
                throw new ConfigurationNotFoundException(nameof(PersonSdsRoleId));
            }
        }
    }
}
