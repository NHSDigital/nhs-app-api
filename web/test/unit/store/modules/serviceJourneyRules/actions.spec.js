import actions from '@/store/modules/serviceJourneyRules/actions';
import { INIT, SET_RULES } from '@/store/modules/serviceJourneyRules/mutation-types';

const createHttp = ({ result = {} } = {}) => ({
  getV1PatientJourneyConfiguration: jest.fn().mockImplementation(() => Promise.resolve(result)),
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
    const rules = { foo: 'test' };

    beforeEach(() => {
      $http = createHttp({ result: rules });
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
