import { initialState } from '@/store/modules/termsAndConditions/mutation-types';

describe('termsAndConditions/mutation-types', () => {
  describe('initial state', () => {
    let state;
    beforeEach(() => {
      state = initialState();
    });

    it('will set `areAccepted` to false', () => {
      expect(state.areAccepted).toEqual(false);
      expect(state.analyticsCookieAccepted).toEqual(false);
      expect(state.updatedConsentRequired).toEqual(false);
    });
  });
});
