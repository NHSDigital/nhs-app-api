import { PRESCRIPTIONS } from '@/lib/routes';

export default {
  hasNoNominatedPharmacy(state) {
    return state.pharmacy.pharmacyName === undefined;
  },
  previousPage(state) {
    return state.previousPageToSearch || PRESCRIPTIONS.path;
  },
  nominatedPharmacyEnabled(state) {
    return state.nominatedPharmacyEnabled;
  },
};
