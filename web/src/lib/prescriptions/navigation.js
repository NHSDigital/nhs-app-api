import { NOMINATED_PHARMACY_CHECK_PATH, PRESCRIPTION_REPEAT_COURSES_PATH } from '@/router/paths';
import sjrIf from '@/lib/sjrIf';

export default function getNavigationPathFromPrescriptions(store) {
  return sjrIf({ $store: store, journey: 'nominatedPharmacy' }) && store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] ?
    NOMINATED_PHARMACY_CHECK_PATH : PRESCRIPTION_REPEAT_COURSES_PATH;
}
