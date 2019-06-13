import { initialState } from '@/store/modules/serviceJourneyRules/mutation-types';

describe('service journey rules mutation types', () => {
  describe('initial state', () => {
    let state;

    beforeEach(() => {
      state = initialState();
    });

    describe('is loaded', () => {
      it('will be false', () => {
        expect(state.isLoaded).toBe(false);
      });
    });
  });
});
