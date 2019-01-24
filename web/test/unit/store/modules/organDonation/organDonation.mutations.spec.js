import mutations from '@/store/modules/organDonation/mutations';
import {
  initialState,
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ACCURACY_ACCEPTANCE,
  SET_ADDITIONAL_DETAILS,
  SET_ALL_ORGANS,
  SET_FAITH_DECLARATION,
  SET_PRIVACY_ACCEPTANCE,
  SET_REGISTRATION_ID,
} from '@/store/modules/organDonation/mutation-types';

describe('organ donation record mutations', () => {
  let state;

  beforeEach(() => {
    state = initialState();
  });

  describe('INIT', () => {
    it('will reset the current state to the default state', () => {
      state.registration.decision = 'foo';
      mutations[INIT](state);
      expect(state).toEqual(initialState());
    });
  });

  describe('LOADED', () => {
    it('will set the organ donation registration state to the received value', () => {
      const { registration } = initialState();
      registration.emailAddress = 'foo@bar';
      mutations[LOADED](state, registration);
      expect(state.registration).toEqual(registration);
      expect(state.registration.emailAddress).toEqual('foo@bar');
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

    it('will set the additional details to its default', () => {
      state.additionalDetails.ethnicityId = 1;
      state.additionalDetails.religionId = 2;

      mutations[MAKE_DECISION](state, 'roar');

      expect(state.additionalDetails).toEqual(initialState().additionalDetails);
    });
  });

  describe('SET_ACCURACY_ACCEPTANCE', () => {
    it('will set the isAccurancyAccepted value to the received value', () => {
      mutations[SET_ACCURACY_ACCEPTANCE](state, true);
      expect(state.isAccuracyAccepted).toEqual(true);
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

  describe('SET_FAITH_DECLARATION', () => {
    it('will set the organ donation registration "faith declaration" state to the received value', () => {
      mutations[SET_FAITH_DECLARATION](state, 'foo');
      expect(state.registration.faithDeclaration).toEqual('foo');
    });
  });

  describe('SET_PRIVACY_ACCEPTANCE', () => {
    it('will set the isPrivacyAccepted value to the received value', () => {
      mutations[SET_PRIVACY_ACCEPTANCE](state, true);
      expect(state.isPrivacyAccepted).toEqual(true);
    });
  });

  describe('SET_REGISTRATION_ID', () => {
    it('will set the organ donation registration identifier state to the received value', () => {
      mutations[SET_REGISTRATION_ID](state, 'identity');
      expect(state.registration.identifier).toEqual('identity');
    });
  });
});
