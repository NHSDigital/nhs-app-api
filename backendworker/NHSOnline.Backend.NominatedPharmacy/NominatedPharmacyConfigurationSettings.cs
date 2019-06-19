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
            string spineCpaId,
            string pdsQueryFromAddress,
            string pdsQueryToAddress,
            int artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
            string partyIdFrom,
            string partyIdTo)
        {
            IsNominatedPharmacyEnabled = isNominatedPharmacyEnabled;
            BaseUrl = baseUrl;
            SpineAccreditedSystemIdFrom = spineAccreditedSystemIdFrom;
            SpineAccreditedSystemIdTo = spineAccreditedSystemIdTo;
            SpineCpaId = spineCpaId;
            PdsQueryFromAddress = pdsQueryFromAddress;
            PdsQueryTo = pdsQueryToAddress;
            ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds = artificialDelayAfterNominatedPharmacyUpdateInMilliseconds;
            PartyIdFrom = partyIdFrom;
            PartyIdTo = partyIdTo;
        }

        public bool IsNominatedPharmacyEnabled { get; }

        public Uri BaseUrl { get; }

        public string SpineAccreditedSystemIdFrom { get; }

        public string SpineAccreditedSystemIdTo { get; }

        public string SpineCpaId { get; }

        public string PdsQueryFromAddress { get; }

        public string PdsQueryTo { get; }
        
        public int ArtificialDelayAfterNominatedPharmacyUpdateInMilliseconds { get; }
        
        public string PartyIdFrom { get; }
        
        public string PartyIdTo { get; }


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

            if (string.IsNullOrEmpty(SpineCpaId))
            {
                throw new ConfigurationNotFoundException(nameof(SpineCpaId));
            }

            if (string.IsNullOrEmpty(PdsQueryFromAddress))
            {
                throw new ConfigurationNotFoundException(nameof(PdsQueryFromAddress));
            }

            if (string.IsNullOrEmpty(PdsQueryTo))
            {
                throw new ConfigurationNotFoundException(nameof(PdsQueryTo));
            }
            
            if (string.IsNullOrEmpty(PartyIdFrom))
            {
                throw new ConfigurationNotFoundException(nameof(PartyIdFrom));
            }
            
            if (string.IsNullOrEmpty(PartyIdTo))
            {
                throw new ConfigurationNotFoundException(nameof(PartyIdTo));
            }
        }
    }
}
