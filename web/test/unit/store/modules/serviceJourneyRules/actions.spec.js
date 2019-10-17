import actions from '@/store/modules/serviceJourneyRules/actions';
import { INIT, SET_RULES } from '@/store/modules/serviceJourneyRules/mutation-types';
import { SET_PATIENT_GUID } from '../../../../../src/store/modules/serviceJourneyRules/mutation-types';

const createHttp = ({ patientJourneyConfigResult = {}, patientConfigResult = {} } = {}) => ({
  getV1PatientJourneyConfiguration: jest.fn().mockImplementation(
    () => Promise.resolve(patientJourneyConfigResult),
  ),
  getV1PatientConfiguration: jest.fn().mockImplementation(
    () => Promise.resolve(patientConfigResult),
  ),
});

describe('service journey rules actions', () => {
  let commit;
  let $http;

  beforeEach(() => {
    commit = jest.fn();
    actions.app = {
      get $http() {
        return $http;
      },
    };
  });

  describe('init', () => {
    beforeEach(() => {
      actions.init({ commit });
    });

    it('will commit INIT', () => {
      expect(commit).toHaveBeenCalledWith(INIT);
    });
  });

  describe('load', () => {
    const rules = {
      journeys: {
        cdssAdmin: {
          provider: 'none',
        },
        cdssAdvice: {
          provider: 'none',
        },
      },
    };

    const patientConfigResponse = {
      response: {
        id: '1234-abcd-5678',
      },
    };

    beforeEach(() => {
      $http = createHttp({
        patientJourneyConfigResult: rules,
        patientConfigResult: patientConfigResponse,
      });
      actions.load({ commit });
    });

    it('will call the `getV1PatientJourneyConfiguration` endpoint', () => {
      expect($http.getV1PatientJourneyConfiguration).toHaveBeenCalled();
    });

    it('will call the `getV1PatientConfiguration` endpoint', () => {
      expect($http.getV1PatientConfiguration).toHaveBeenCalled();
    });

    it('will commit SET_RULES passing in the rules', () => {
      expect(commit).toHaveBeenCalledWith(SET_RULES, rules);
    });

    it('will commit SET_PATIENT_GUID passing in the patientGuid', () => {
      expect(commit).toHaveBeenCalledWith(SET_PATIENT_GUID, patientConfigResponse.response.id);
    });
  });
});
