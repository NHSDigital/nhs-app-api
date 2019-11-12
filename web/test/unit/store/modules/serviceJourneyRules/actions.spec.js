import actions from '@/store/modules/serviceJourneyRules/actions';
import { INIT, SET_RULES } from '@/store/modules/serviceJourneyRules/mutation-types';

const createHttp = ({ patientJourneyConfigResult = {} } = {}) => ({
  getV1PatientJourneyConfiguration: jest.fn().mockImplementation(
    () => Promise.resolve(patientJourneyConfigResult),
  ),
  getV1PatientLinkedAccountJourneyConfiguration: jest.fn().mockImplementation(
    () => Promise.resolve(patientJourneyConfigResult),
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

    beforeEach(() => {
      $http = createHttp({
        patientJourneyConfigResult: rules,
      });
      actions.load({ commit });
    });

    it('will call the `getV1PatientJourneyConfiguration` endpoint', () => {
      expect($http.getV1PatientJourneyConfiguration).toHaveBeenCalled();
    });

    it('will commit SET_RULES passing in the rules', () => {
      expect(commit).toHaveBeenCalledWith(SET_RULES, rules);
    });
  });

  describe('loadLinkedAccount', () => {
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

    beforeEach(() => {
      $http = createHttp({
        patientJourneyConfigResult: rules,
      });
      actions.loadLinkedAccount({ commit });
    });

    it('will call the `getV1PatientLinkedAccountJourneyConfiguration` endpoint', () => {
      expect($http.getV1PatientLinkedAccountJourneyConfiguration).toHaveBeenCalled();
    });

    it('will commit SET_RULES passing in the rules', () => {
      expect(commit).toHaveBeenCalledWith(SET_RULES, rules);
    });
  });
});
