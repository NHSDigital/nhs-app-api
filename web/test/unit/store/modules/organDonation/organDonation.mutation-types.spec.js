import { initialState } from '@/store/modules/organDonation/mutation-types';

describe('organ donation mutation types', () => {
  describe('initial state', () => {
    let state;
    beforeEach(() => {
      state = initialState();
    });

    describe('originalRegistration', () => {
      it('should exist', () => {
        expect(state.originalRegistration).not.toBeUndefined();
      });

      it('should be a clone of the registration object', () => {
        expect(state.originalRegistration).toEqual(state.registration);
      });
    });
  });
});
