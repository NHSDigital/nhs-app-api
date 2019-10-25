import actions from '@/store/modules/linkedAccounts/actions';
import { SET_LINKED_ACCOUNTS_CONFIG } from '@/store/modules/linkedAccounts/mutation-types';

const {
  initialiseConfig,
} = actions;

describe('initialiseConfig', () => {
  it('will call the API to retrieve linked accounts then call commit to add to the store', () => {
    const configResponse = {
      response: {
        config: {
          patientId: '1234-abcd-5678',
          hasLinkedAccounts: true,
        },
      },
    };

    const that = {
      app: {
        $http: {
          getV1PatientConfiguration:
            jest.fn().mockImplementation(() => Promise.resolve(configResponse)),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();
    return initialiseConfig
      .call(that, { commit })
      .then(() => {
        expect(that.app.$http.getV1PatientConfiguration).toHaveBeenCalled();
        expect(commit).toHaveBeenCalledWith(SET_LINKED_ACCOUNTS_CONFIG, configResponse.response);
      });
  });
});

