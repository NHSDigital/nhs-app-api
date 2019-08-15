import actions from '@/store/modules/notifications/actions';
import { SET_REGISTRATION, SET_WAITING } from '@/store/modules/notifications/mutation-types';

describe('notifications actions', () => {
  let $http;
  let commit;

  beforeEach(() => {
    commit = jest.fn();
    $http = {
      postV1ApiUsersDevices: jest.fn().mockImplementation(() => Promise.resolve()),
    };
    actions.app = {
      get $http() {
        return $http;
      },
    };
    global.nativeApp = {
      requestPnsToken: jest.fn(),
    };
  });

  describe('authorised', () => {
    const deviceResponse = { test: 5 };
    const state = { registered: true };

    beforeEach(async () => {
      await actions.authorised({ commit, state }, JSON.stringify(deviceResponse));
    });

    it('will call the `postV1ApiUsersDevices` endpoint', () => {
      expect($http.postV1ApiUsersDevices).toBeCalledWith({
        addDeviceRequest: deviceResponse,
      });
    });

    it('will commit a value of `false` to `SET_WAITING`', () => {
      expect(commit).toBeCalledWith(SET_WAITING, false);
    });

    it('will commit the inverse of registered to `SET_REGISTRATION`', () => {
      expect(commit).toBeCalledWith(SET_REGISTRATION, !state.registered);
    });
  });

  describe('toggle', () => {
    beforeEach(() => {
      actions.toggle({ commit });
    });

    it('will commit a value of `true` to `SET_WAITING`', () => {
      expect(commit).toBeCalledWith(SET_WAITING, true);
    });

    it('will call native app `requestPnsToken`', () => {
      expect(global.nativeApp.requestPnsToken).toBeCalled();
    });
  });

  describe('unAuthorised', () => {
    beforeEach(() => {
      actions.unAuthorised({ commit });
    });

    it('will commit a value of `false` to `SET_WAITING`', () => {
      expect(commit).toBeCalledWith(SET_WAITING, false);
    });
  });
});
