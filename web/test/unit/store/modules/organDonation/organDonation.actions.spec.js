import actions from '@/store/modules/organDonation/actions';
import { LOADED, MAKE_DECISION } from '@/store/modules/organDonation/mutation-types';

const createHttp = (result = {}) => ({
  getV1PatientOrgandonation: jest.fn().mockImplementation(() => Promise.resolve(result)),
});

describe('organ donation actions', () => {
  let $http;
  let commit;
  let result;

  beforeEach(() => {
    result = { mock: result };
    commit = jest.fn();
    $http = createHttp(result);
    actions.app = {
      $http,
    };
  });

  describe('acceptDonation', () => {
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
});
