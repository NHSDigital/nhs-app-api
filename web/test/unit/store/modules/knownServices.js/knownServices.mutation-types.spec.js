import { initialState } from '@/store/modules/knownServices/mutation-types';

describe('known services mutation types', () => {
  describe('initial state', () => {
    let state;

    beforeEach(() => {
      state = initialState();
    });

    it('will have nothing in the known services array', () => {
      expect(Array.isArray(state.knownServices)).toBe(true);
      expect(state.knownServices.length).toBe(0);
    });

    it('will not have loaded yet', () => {
      expect(state.isLoaded).toBe(false);
    });
  });
});
