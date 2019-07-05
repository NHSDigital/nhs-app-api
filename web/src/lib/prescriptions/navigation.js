import { NOMINATED_PHARMACY_CHECK, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';

export default function getNavigationPathFromPrescriptions(store) {
  return store.getters['serviceJourneyRules/nominatedPharmacyEnabled'] && store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] ?
    NOMINATED_PHARMACY_CHECK.path : PRESCRIPTION_REPEAT_COURSES.path;
}
