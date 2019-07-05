import getters from '@/store/modules/nominatedPharmacy/getters';
import { initialState } from '@/store/modules/nominatedPharmacy/mutation-types';

describe('getters', () => {
  let currentState;

  beforeEach(() => {
    currentState = initialState;
  });

  describe('nominatedPharmacyEnabled', () => {
    const { nominatedPharmacyEnabled } = getters;

    it('will be false if state nominated pharmacy enabled is false', () => {
      currentState.nominatedPharmacyEnabled = false;
      expect(nominatedPharmacyEnabled(currentState)).toBe(false);
    });

    it('will be true if state nominated pharmacy enabled is true', () => {
      currentState.nominatedPharmacyEnabled = true;
      expect(nominatedPharmacyEnabled(currentState)).toBe(true);
    });
  });
});
