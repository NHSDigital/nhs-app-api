import mutations from '@/store/modules/organDonation/mutations';
import {
  initialState,
  LOADED,
  LOADED_REFERENCE_DATA,
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

  describe('LOADED_REFERENCE_DATA', () => {
    it('will set the organ donation referenceData state to the received value', () => {
      const data = { ethnicities: [], genders: [] };
      mutations[LOADED_REFERENCE_DATA](state, data);
      expect(state.referenceData).toEqual(data);
    });
  });

  describe('MAKE_DECISION', () => {
    it('will set the organ donation decision', () => {
      mutations[MAKE_DECISION](state, 'roar');
      expect(state.registration.decision).toEqual('roar');
    });
  });
});
