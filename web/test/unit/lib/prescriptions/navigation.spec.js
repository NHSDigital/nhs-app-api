import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import { NOMINATED_PHARMACY_CHECK_PATH, PRESCRIPTION_REPEAT_COURSES_PATH } from '@/router/paths';

describe('navigation', () => {
  let getters;
  let store;

  beforeEach(() => {
    getters = {};
    store = {
      getters,
    };
  });

  describe('getNavigationPathFromPrescriptions', () => {
    describe('sjr nominated pharmacy enabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/nominatedPharmacyEnabled'] = true;
      });

      describe('state nominated pharmacy enabled', () => {
        beforeEach(() => {
          getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
        });

        it('will be nominated pharmacy check path', () => {
          expect(GetNavigationPathFromPrescriptions(store)).toBe(NOMINATED_PHARMACY_CHECK_PATH);
        });
      });

      describe('state nominated pharmacy disabled', () => {
        beforeEach(() => {
          getters['nominatedPharmacy/nominatedPharmacyEnabled'] = false;
        });

        it('will be prescriptions repeat courses path', () => {
          expect(GetNavigationPathFromPrescriptions(store)).toBe(PRESCRIPTION_REPEAT_COURSES_PATH);
        });
      });
    });


    describe('sjr nominated pharmacy disabled', () => {
      beforeEach(() => {
        getters['serviceJourneyRules/nominatedPharmacyEnabled'] = false;
      });

      describe('state nominated pharmacy enabled', () => {
        beforeEach(() => {
          getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
        });

        it('will be prescriptions repeat courses path', () => {
          expect(GetNavigationPathFromPrescriptions(store)).toBe(PRESCRIPTION_REPEAT_COURSES_PATH);
        });
      });

      describe('state nominated pharmacy disabled', () => {
        beforeEach(() => {
          getters['nominatedPharmacy/nominatedPharmacyEnabled'] = false;
        });

        it('will be prescriptions repeat courses path', () => {
          expect(GetNavigationPathFromPrescriptions(store)).toBe(PRESCRIPTION_REPEAT_COURSES_PATH);
        });
      });
    });
  });
});
