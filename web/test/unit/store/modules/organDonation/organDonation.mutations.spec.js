import mutations from '@/store/modules/organDonation/mutations';
import {
  initialState,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ALL_ORGANS,
  SET_ADDITIONAL_DETAILS,
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

  describe('SET_ALL_ORGANS', () => {
    it('will set the organ donation registration decision "all" state to the received value', () => {
      mutations[SET_ALL_ORGANS](state, false);
      expect(state.registration.decisionDetails.all).toEqual(false);
    });
  });

  describe('SET_ADDITIONAL_DETAILS', () => {
    beforeEach(() => {
      mutations[SET_ADDITIONAL_DETAILS](state, {
        ethnicityId: 'foo',
        religionId: 'bar',
      });
    });

    it('will set the ethnicityId on the state', () => {
      expect(state.additionalDetails.ethnicityId).toEqual('foo');
    });

    it('will set the religionId on the state', () => {
      expect(state.additionalDetails.religionId).toEqual('bar');
    });
  });
});
