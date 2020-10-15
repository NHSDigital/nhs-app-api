import get from 'lodash/fp/get';
import difference from 'lodash/fp/difference';
import intersection from 'lodash/fp/intersection';
import medicalRecordRoutes, {
  HEALTH_RECORDS,
  GP_MEDICAL_RECORD,
  UPLIFT_GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
} from '@/router/routes/medical-record';
import gpMedicalRecordAcceptance from '@/middleware/gpMedicalRecordAcceptance';

const excludedRoutes = [
  HEALTH_RECORDS,
  GP_MEDICAL_RECORD,
  UPLIFT_GP_MEDICAL_RECORD,
  GP_MEDICAL_RECORD_GP_AT_HAND,
];

const acceptanceRequiredRoutes = difference(medicalRecordRoutes, excludedRoutes);
const acceptanceNotRequiredRoutes = intersection(medicalRecordRoutes, excludedRoutes);

describe('medical record routes', () => {
  describe('routes requiring terms acceptance', () => {
    it.each(
      acceptanceRequiredRoutes,
    )('will have a gpMedicalRecordAcceptance middleware', (route) => {
      expect(get('meta.middleware', route) || []).toContain(gpMedicalRecordAcceptance);
    });
  });
  describe('routes not requiring terms acceptance', () => {
    it.each(
      acceptanceNotRequiredRoutes,
    )('will not have a gpMedicalRecordAcceptance middleware', (route) => {
      expect(get('meta.middleware', route) || []).not.toContain(gpMedicalRecordAcceptance);
    });
  });
});
