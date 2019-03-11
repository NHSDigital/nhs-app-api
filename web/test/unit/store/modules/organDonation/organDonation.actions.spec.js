import actions from '@/store/modules/organDonation/actions';
import {
  CLONE_FROM_ORIGINAL,
  INIT,
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  RESET_REGISTRATION,
  SET_ACCURACY_ACCEPTANCE,
  SET_ADDITIONAL_DETAILS,
  SET_ALL_ORGANS,
  SET_AMENDING,
  SET_FAITH_DECLARATION,
  SET_PRIVACY_ACCEPTANCE,
  SET_REAFFIRMING,
  SET_REGISTRATION_ID,
  SET_STATE,
  SET_WITHDRAW_REASON_ID,
  SET_WITHDRAWING,
  STATE_OK,
  UPDATE_ORIGINAL_REGISTRATION,
} from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_VIEW_DECISION, ORGAN_DONATION_WITHDRAWN } from '@/lib/routes';
import { createRouter } from '../../../helpers';

const createHttp = ({
  result = {},
  referenceData = {},
  identifier = 'boo',
  state = STATE_OK,
} = {}) => ({
  deleteV1PatientOrgandonation: jest.fn().mockImplementation(() => Promise.resolve()),
  getV1PatientOrgandonation: jest.fn().mockImplementation(() => Promise.resolve(result)),
  getV1PatientOrgandonationReferencedata:
    jest
      .fn()
      .mockImplementation(() => Promise.resolve(referenceData)),
  postV1PatientOrgandonation:
    jest
      .fn()
      .mockImplementation(() => Promise.resolve({ identifier, state })),
  putV1PatientOrgandonation:
    jest
      .fn()
      .mockImplementation(() => Promise.resolve({ identifier, state })),
});

describe('organ donation actions', () => {
  let $http;
  let commit;
  let result;
  let referenceData;

  beforeEach(() => {
    result = 'result';
    referenceData = 'reference-data';
    commit = jest.fn();
    $http = createHttp({ result, referenceData });
    actions.app = {
      get $http() {
        return $http;
      },
    };
  });

  describe('amendCancel', () => {
    beforeEach(() => {
      actions.amendCancel({ commit, state: {} });
    });

    it('will commit a value of "false" to "SET_AMENDING"', () => {
      expect(commit).toHaveBeenCalledWith(SET_AMENDING, false);
    });
  });

  describe('amendStart', () => {
    beforeEach(() => {
      actions.amendStart({
        commit,
        state: {},
      });
    });

    it('will commit a value of "true" to "SET_AMENDING"', () => {
      expect(commit).toHaveBeenCalledWith(SET_AMENDING, true);
    });

    it('will commit "RESET_REGISTRATION"', () => {
      expect(commit).toHaveBeenCalledWith(RESET_REGISTRATION);
    });

    it('will update the registration with data from the original registration', () => {
      expect(commit).toHaveBeenCalledWith(CLONE_FROM_ORIGINAL, [
        'identifier',
        'nameFull',
        'nhsNumber',
        'name',
        'gender',
        'dateOfBirth',
        'addressFull',
        'address',
        'emailAddress',
      ]);
    });
  });

  describe('cloneFromOriginal', () => {
    beforeEach(() => {
      actions.cloneFromOriginal({ commit }, 'identifier');
    });

    it('will commit the CLONE_FROM_ORIGINAL mutation passing the received path', () => {
      expect(commit).toHaveBeenCalledWith(CLONE_FROM_ORIGINAL, 'identifier');
    });
  });

  describe('deleteRegistration', () => {
    let state;

    beforeEach(async () => {
      $http = createHttp();
      state = {
        withdrawReasonId: 'withdraw reason id',
        originalRegistration: { nhsNumber: '12345' },
      };

      actions.$router = createRouter();
      await actions.deleteRegistration({ commit, state });
    });

    it('will call the `deleteV1PatientOrgandonation` endpoint', () => {
      expect($http.deleteV1PatientOrgandonation).toHaveBeenCalledWith({
        organDonationWithdrawRequest: {
          ...state.originalRegistration,
          withdrawReasonId: state.withdrawReasonId,
        },
      });
    });

    it('will commit INIT', () => {
      expect(commit).toHaveBeenCalledWith(INIT);
    });

    it('will push organ donation withdrawn to the router', () => {
      expect(actions.$router.push).toHaveBeenCalledWith(ORGAN_DONATION_WITHDRAWN.path);
    });
  });

  describe('getReferenceData', () => {
    it('will request the organ donation reference data', () => {
      actions.getReferenceData({ commit });
      expect($http.getV1PatientOrgandonationReferencedata).toHaveBeenCalled();
    });

    it('will commit the reference data on completion', async () => {
      await actions.getReferenceData({ commit });
      expect(commit).toHaveBeenCalledWith(LOADED_REFERENCE_DATA, referenceData);
    });
  });

  describe('getRegistration', () => {
    it('will request the organ donation registration', () => {
      actions.getRegistration({ commit });
      expect($http.getV1PatientOrgandonation).toHaveBeenCalled();
    });

    it('will commit the result on completion', async () => {
      await actions.getRegistration({ commit });
      expect(commit).toHaveBeenCalledWith(LOADED, result);
    });
  });

  describe('makeDecision', () => {
    it('will commit the MAKE_DECISION mutation', () => {
      actions.makeDecision({ commit }, 'foo');
      expect(commit).toHaveBeenCalledWith(MAKE_DECISION, 'foo');
    });
  });

  describe('resetAcceptanceChecks', () => {
    it(
      'will commit SET_PRIVACY_ACCEPTANCE and SET_ACCURACY_ACCEPTANCE with a value of false',
      () => {
        actions.resetAcceptanceChecks({ commit });
        expect(commit).toHaveBeenCalledWith(SET_PRIVACY_ACCEPTANCE, false);
        expect(commit).toHaveBeenCalledWith(SET_ACCURACY_ACCEPTANCE, false);
      },
    );
  });

  describe('setAdditionalDetails', () => {
    it('will commit the additional details', () => {
      const additionalDetails = {
        selectedEthnicity: { id: 1, displayName: 'one' },
        selectedReligion: { id: 2, displayName: 'two' },
      };

      actions.setAdditionalDetails({ commit }, additionalDetails);
      expect(commit).toHaveBeenCalledWith(SET_ADDITIONAL_DETAILS, additionalDetails);
    });
  });

  describe('setAllOrgans', () => {
    it('will commit the SET_ALL_ORGANS mutation', () => {
      actions.setAllOrgans({ commit }, true);
      expect(commit).toHaveBeenCalledWith(SET_ALL_ORGANS, true);
    });
  });

  describe('setFaithDeclaration', () => {
    it('will commit the faith declaration', () => {
      actions.setFaithDeclaration({ commit }, 'Yes');
      expect(commit).toHaveBeenCalledWith(SET_FAITH_DECLARATION, 'Yes');
    });
  });

  describe('submitDecision', () => {
    let dispatch;
    let state;

    beforeEach(async () => {
      dispatch = jest.fn();
    });

    describe('withdrawing', () => {
      beforeEach(async () => {
        state = { isWithdrawing: true };
        await actions.submitDecision({ dispatch, state });
      });

      it('will dispatch "deleteRegistration"', () => {
        expect(dispatch).toHaveBeenCalledWith('deleteRegistration');
      });
    });

    describe('not withdrawing', () => {
      beforeEach(async () => {
        state = { isWithdrawing: false };
        await actions.submitDecision({ dispatch, state });
      });

      it('will dispatch "submitRegistration"', () => {
        expect(dispatch).toHaveBeenCalledWith('submitRegistration');
      });
    });
  });

  describe('submitRegistration', () => {
    let state;
    let expectedIdentifier;
    let expectedState;

    beforeEach(() => {
      expectedIdentifier = '999';
      expectedState = STATE_OK;
      $http = createHttp({
        result,
        referenceData,
        identifier: expectedIdentifier,
        state: expectedState,
      });

      state = {
        additionalDetails: 'additional details',
        registration: { nhsNumber: '12345' },
      };

      actions.$router = createRouter();
    });

    describe('is amending', () => {
      beforeEach(async () => {
        state.isAmending = true;
        await actions.submitRegistration({ commit, state });
      });

      it('will call the `putV1PatientOrgandonation` endpoint', () => {
        expect($http.putV1PatientOrgandonation).toHaveBeenCalledWith({
          organDonationRegistrationRequest: {
            additionalDetails: state.additionalDetails,
            registration: state.registration,
          },
        });
      });

      it('will commit the returned identifier using the SET_REGISTRATION_ID mutation type', () => {
        expect(commit).toHaveBeenCalledWith(SET_REGISTRATION_ID, expectedIdentifier);
      });

      it('will commit the returned state using the SET_STATE mutation type', () => {
        expect(commit).toHaveBeenCalledWith(SET_STATE, expectedState);
      });

      it('will commit the UPDATE_ORIGINAL_REGISTRATION mutation type', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_ORIGINAL_REGISTRATION);
      });

      it('will push organ donation view decision to the router', () => {
        expect(actions.$router.push).toHaveBeenCalledWith(ORGAN_DONATION_VIEW_DECISION.path);
      });
    });

    describe('is reaffirming', () => {
      beforeEach(async () => {
        state.isReaffirming = true;
        await actions.submitRegistration({ commit, state });
      });

      it('will call the `putV1PatientOrgandonation` endpoint', () => {
        expect($http.putV1PatientOrgandonation).toHaveBeenCalledWith({
          organDonationRegistrationRequest: {
            additionalDetails: state.additionalDetails,
            registration: state.registration,
          },
        });
      });

      it('will commit the returned identifier using the SET_REGISTRATION_ID mutation type', () => {
        expect(commit).toHaveBeenCalledWith(
          SET_REGISTRATION_ID,
          expectedIdentifier,
        );
      });

      it('will commit the returned state using the SET_STATE mutation type', () => {
        expect(commit).toHaveBeenCalledWith(SET_STATE, expectedState);
      });

      it('will commit the UPDATE_ORIGINAL_REGISTRATION mutation type', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_ORIGINAL_REGISTRATION);
      });

      it('will push organ donation view decision to the router', () => {
        expect(actions.$router.push).toHaveBeenCalledWith(ORGAN_DONATION_VIEW_DECISION.path);
      });
    });

    describe('new registration', () => {
      beforeEach(async () => {
        state.isAmending = false;
        state.isReaffirming = false;
        await actions.submitRegistration({ commit, state });
      });

      it('will post to the `postV1PatientOrgandonation` endpoint', () => {
        expect($http.postV1PatientOrgandonation).toHaveBeenCalledWith({
          organDonationRegistrationRequest: {
            additionalDetails: state.additionalDetails,
            registration: state.registration,
          },
        });
      });

      it('will commit the returned state using the SET_STATE mutation type', () => {
        expect(commit).toHaveBeenCalledWith(SET_STATE, expectedState);
      });

      it('will commit the returned identifier using the SET_REGISTRATION_ID mutation type', () => {
        expect(commit).toHaveBeenCalledWith(SET_REGISTRATION_ID, expectedIdentifier);
      });

      it('will commit the UPDATE_ORIGINAL_REGISTRATION mutation type', () => {
        expect(commit).toHaveBeenCalledWith(UPDATE_ORIGINAL_REGISTRATION);
      });

      it('will push organ donation view decision to the router', () => {
        expect(actions.$router.push).toHaveBeenCalledWith(ORGAN_DONATION_VIEW_DECISION.path);
      });
    });
  });

  describe('reaffirmCancel', () => {
    beforeEach(() => {
      actions.reaffirmCancel({ commit, state: {} });
    });

    it('will commit a value of "false" to "SET_REAFFIRMING"', () => {
      expect(commit).toHaveBeenCalledWith(SET_REAFFIRMING, false);
    });
  });

  describe('reaffirmStart', () => {
    beforeEach(() => {
      actions.reaffirmStart({ commit, state: {} });
    });

    it('will commit a value of "true" to "SET_REAFFIRMING"', () => {
      expect(commit).toHaveBeenCalledWith(SET_REAFFIRMING, true);
    });
  });

  describe('setWithdrawReasonId', () => {
    beforeEach(() => {
      actions.setWithdrawReasonId({ commit }, 'Other');
    });

    it('will commit a value of "Other" to "SET_WITHDRAW_REASON_ID"', () => {
      expect(commit).toHaveBeenCalledWith(SET_WITHDRAW_REASON_ID, 'Other');
    });
  });

  describe('toggleAccuracyAcceptance', () => {
    it(
      'will commit SET_ACCURACY_ACCEPTANCE with a value of true when `isAccuracyAcceptance` is false',
      () => {
        const state = { isAccuracyAccepted: false };
        actions.toggleAccuracyAcceptance({ commit, state });
        expect(commit).toHaveBeenCalledWith(SET_ACCURACY_ACCEPTANCE, true);
      },
    );

    it(
      'will commit SET_ACCURACY_ACCEPTANCE with a value of false when `isAccuracyAcceptance` is true',
      () => {
        const state = { isAccuracyAccepted: true };
        actions.toggleAccuracyAcceptance({ commit, state });
        expect(commit).toHaveBeenCalledWith(SET_ACCURACY_ACCEPTANCE, false);
      },
    );
  });

  describe('togglePrivacyAcceptance', () => {
    it(
      'will commit SET_PRIVACY_ACCEPTANCE with a value of true when `isPrivacyAcceptance` is false',
      () => {
        const state = { isPrivacyAccepted: false };
        actions.togglePrivacyAcceptance({ commit, state });
        expect(commit).toHaveBeenCalledWith(SET_PRIVACY_ACCEPTANCE, true);
      },
    );

    it(
      'will commit SET_PRIVACY_ACCEPTANCE with a value of false when `isPrivacyAcceptance` is true',
      () => {
        const state = { isPrivacyAccepted: true };
        actions.togglePrivacyAcceptance({ commit, state });
        expect(commit).toHaveBeenCalledWith(SET_PRIVACY_ACCEPTANCE, false);
      },
    );
  });

  describe('withdrawCancel', () => {
    beforeEach(() => {
      actions.withdrawCancel({ commit, state: {} });
    });

    it('will commit a value of "false" to "SET_WITHDRAWING"', () => {
      expect(commit).toHaveBeenCalledWith(SET_WITHDRAWING, false);
    });

    it('will commit a value of "" to "SET_WITHDRAW_REASON_ID"', () => {
      expect(commit).toHaveBeenCalledWith(SET_WITHDRAW_REASON_ID, '');
    });
  });

  describe('withdrawStart', () => {
    beforeEach(() => {
      actions.withdrawStart({ commit, state: {} });
    });

    it('will commit a value of "true" to "SET_WITHDRAWING"', () => {
      expect(commit).toHaveBeenCalledWith(SET_WITHDRAWING, true);
    });
  });
});
