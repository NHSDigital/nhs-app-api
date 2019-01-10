import actions from '@/store/modules/organDonation/actions';
import {
  LOADED,
  LOADED_REFERENCE_DATA,
  MAKE_DECISION,
  SET_ALL_ORGANS,
  SET_ADDITIONAL_DETAILS,
} from '@/store/modules/organDonation/mutation-types';

const createHttp = ({ result = {}, referenceData = {} } = {}) => ({
  getV1PatientOrgandonation: jest.fn().mockImplementation(() => Promise.resolve(result)),
  getV1PatientOrgandonationReferencedata:
    jest
      .fn()
      .mockImplementation(() => Promise.resolve(referenceData)),
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
      $http,
    };
  });

  describe('makeDecision', () => {
    it('will commit the MAKE_DECISION mutation', () => {
      actions.makeDecision({ commit }, 'foo');
      expect(commit).toHaveBeenCalledWith(MAKE_DECISION, 'foo');
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

  describe('setAllOrgans', () => {
    it('will commit the SET_ALL_ORGANS mutation', () => {
      actions.setAllOrgans({ commit }, true);
      expect(commit).toHaveBeenCalledWith(SET_ALL_ORGANS, true);
    });
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
});
