import {
  NOMINATED_PHARMACY_CHECK_PATH,
  PRESCRIPTION_TYPE_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
} from '@/router/paths';
import sjrIf from '@/lib/sjrIf';

export function getNavigationPathFromPrescriptionType(store) {
  return sjrIf({ $store: store, journey: 'nominatedPharmacy' }) && store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] ?
    NOMINATED_PHARMACY_CHECK_PATH : PRESCRIPTION_REPEAT_COURSES_PATH;
}

export function getNavigationPathFromPrescription() {
  return PRESCRIPTION_TYPE_PATH;
}
