import actions from '@/store/modules/serviceJourneyRules/actions';
import { INIT, SET_RULES } from '@/store/modules/serviceJourneyRules/mutation-types';

const createHttp = ({ result = {} } = {}) => ({
  getV1PatientJourneyConfiguration: jest.fn().mockImplementation(
    () => Promise.resolve(result),
  ),
});

const createCds = result => ({
  getFhirServiceDefinitionProviderNameByProvider: jest.fn().mockImplementation(
    () => Promise.resolve(result),
  ),
});

describe('service journey rules actions', () => {
  let commit;
  let $http;
  let $cdsApi;

  beforeEach(() => {
    commit = jest.fn();
    actions.app = {
      get $http() {
        return $http;
      },
      get $cdsApi() {
        return $cdsApi;
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
      $http = createHttp({ result: rules });
      $cdsApi = createCds('eConsult Health Ltd');
      actions.load({ commit });
    });

    it('will call the `getV1PatientJourneyConfiguration` endpoint', () => {
      expect($http.getV1PatientJourneyConfiguration).toHaveBeenCalled();
    });

    it('will commit SET_RULES passing in the rules', () => {
      expect(commit).toHaveBeenCalledWith(SET_RULES, rules);
    });
  });
});
