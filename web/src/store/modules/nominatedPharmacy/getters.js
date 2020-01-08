export default {
  hasNoNominatedPharmacy(state) {
    return state.pharmacy.pharmacyName === undefined;
  },
  nominatedPharmacyEnabled(state) {
    return state.nominatedPharmacyEnabled;
  },
  justUpdated(state) {
    return state.justUpdated;
  },
  pharmacyName(state) {
    return state.pharmacy.pharmacyName;
  },
  getOnlineOnlyKnownOption(state) {
    return state.onlineOnlyKnownOption;
  },
};
