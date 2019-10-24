import actions from '@/store/modules/linkedAccounts/actions';
import {
  LOADED,
  SWITCH_TO_LINKED_ACCOUNT,
  LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY,
} from '@/store/modules/linkedAccounts/mutation-types';

const {
  load,
  loadAccountAccessSummary,
  switchProfile,
} = actions;

describe('load', () => {
  it('will call the API to retrieve linked accounts then call commit to add to the store', () => {
    const profiles = [{
      name: 'john',
    }, {
      name: 'ben',
    }];

    const that = {
      app: {
        $http: {
          getV1PatientLinkedAccounts: jest.fn().mockImplementation(() => Promise.resolve(profiles)),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();

    return load
      .call(that, { commit })
      .then(() => {
        expect(that.app.$http.getV1PatientLinkedAccounts).toHaveBeenCalled();
        expect(commit).toHaveBeenCalledWith(LOADED, profiles);
        expect(that.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
      });
  });
});

describe('loadAccountAccessSummary', () => {
  it('will call the API and then call commit with the returned data on success', () => {
    const id = '23283891';
    const profileData = {
      name: 'john',
    };

    const that = {
      app: {
        $http: {
          getV1PatientLinkedAccountsAccessSummary: jest.fn().mockImplementation(
            () => Promise.resolve(profileData),
          ),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();

    return loadAccountAccessSummary
      .call(that, { commit }, id)
      .then(() => {
        expect(that.app.$http.getV1PatientLinkedAccountsAccessSummary).toHaveBeenCalledWith({ id });
        expect(commit).toHaveBeenCalledWith(LOADED_LINKED_ACCOUNT_ACCESS_SUMMARY, profileData);
        expect(that.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
      });
  });
});

describe('switchProfile', () => {
  it('will call the API and then call commit with the passed in profile on success', () => {
    const profile = {
      id: 'id-value',
    };

    const that = {
      app: {
        $http: {
          postV1PatientLinkedAccountsSwitchById: jest.fn().mockImplementation(
            () => Promise.resolve(),
          ),
        },
      },
    };

    const commit = jest.fn();

    return switchProfile
      .call(that, { commit }, profile)
      .then(() => {
        expect(that.app.$http.postV1PatientLinkedAccountsSwitchById).toHaveBeenCalled();
        expect(commit).toHaveBeenCalledWith(SWITCH_TO_LINKED_ACCOUNT, profile);
      });
  });
});
