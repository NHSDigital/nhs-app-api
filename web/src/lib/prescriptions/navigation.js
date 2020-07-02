import { NOMINATED_PHARMACY_CHECK, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';
import sjrIf from '@/lib/sjrIf';

export default function getNavigationPathFromPrescriptions(store) {
  return sjrIf({ $store: store, journey: 'nominatedPharmacy' }) && store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] ?
    NOMINATED_PHARMACY_CHECK.path : PRESCRIPTION_REPEAT_COURSES.path;
}
