export default {
  hasNoNominatedPharmacy(state) {
    return state.pharmacy.pharmacyName === undefined;
  },
};
