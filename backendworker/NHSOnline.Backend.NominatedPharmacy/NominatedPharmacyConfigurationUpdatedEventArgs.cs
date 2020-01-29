using System;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class NominatedPharmacyConfigurationUpdatedEventArgs : EventArgs
    {
        public INominatedPharmacyConfigurationSettings Config { get; set; }
    }
}
