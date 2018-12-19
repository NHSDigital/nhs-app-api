import mutations from '@/store/modules/organDonation/mutations';
import {
  initialState,
  LOADED,
  MAKE_DECISION,
} from '@/store/modules/organDonation/mutation-types';

describe('organ donation record mutations', () => {
  let state;

  beforeEach(() => {
    state = initialState();
  });

  describe('LOADED', () => {
    it('will set the organ donation registration state to the received value', () => {
      const data = { registration: 'from service' };
      mutations[LOADED](state, data);
      expect(state.registration).toEqual(data);
    });
  });

  describe('MAKE_DECISION', () => {
    it('will set the organ donation decision', () => {
      mutations[MAKE_DECISION](state, 'roar');
      expect(state.registration.decision).toEqual('roar');
    });
  });
});
