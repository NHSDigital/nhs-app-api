import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import { NOMINATED_PHARMACY_CHECK, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';

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
          expect(GetNavigationPathFromPrescriptions(store)).toBe(NOMINATED_PHARMACY_CHECK.path);
        });
      });

      describe('state nominated pharmacy disabled', () => {
        beforeEach(() => {
          getters['nominatedPharmacy/nominatedPharmacyEnabled'] = false;
        });

        it('will be prescriptions repeat courses path', () => {
          expect(GetNavigationPathFromPrescriptions(store)).toBe(PRESCRIPTION_REPEAT_COURSES.path);
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
          expect(GetNavigationPathFromPrescriptions(store)).toBe(PRESCRIPTION_REPEAT_COURSES.path);
        });
      });

      describe('state nominated pharmacy disabled', () => {
        beforeEach(() => {
          getters['nominatedPharmacy/nominatedPharmacyEnabled'] = false;
        });

        it('will be prescriptions repeat courses path', () => {
          expect(GetNavigationPathFromPrescriptions(store)).toBe(PRESCRIPTION_REPEAT_COURSES.path);
        });
      });
    });
  });
});
