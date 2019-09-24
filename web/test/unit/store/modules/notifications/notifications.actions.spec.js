import actions from '@/store/modules/notifications/actions';
import { SET_REGISTRATION, SET_SETTINGS_ENABLED, SET_WAITING } from '@/store/modules/notifications/mutation-types';

describe('notifications actions', () => {
  let getSuccess;
  let $http;
  let commit;

  const promiseReturn = (success) => {
    if (success) {
      return Promise.resolve();
    }
    return Promise.reject();
  };

  beforeEach(() => {
    getSuccess = true;
    commit = jest.fn();
    $http = {
      deleteV1ApiUsersDevices: jest.fn().mockImplementation(() => Promise.resolve()),
      getV1ApiUsersDevices: jest.fn().mockImplementation(() => promiseReturn(getSuccess)),
      postV1ApiUsersDevices: jest.fn().mockImplementation(() => Promise.resolve()),
    };
    actions.app = {
      get $http() {
        return $http;
      },
    };
    global.nativeApp = {
      areNotificationsEnabled: jest.fn(),
      requestPnsToken: jest.fn(),
    };
  });

  describe('authorised', () => {
    const state = { registered: false };
    const deviceResponse = { deviceType: 'android', devicePns: 5, trigger: 'toggle' };

    describe('on load', () => {
      beforeEach(() => {
        deviceResponse.trigger = 'load';
      });

      describe('found', () => {
        beforeEach(async () => {
          getSuccess = true;
          await actions.authorised({ commit, state }, JSON.stringify(deviceResponse));
        });

        it('will call the `getV1ApiUsersDevices` endpoint', () => {
          expect($http.getV1ApiUsersDevices).toBeCalledWith(
            { devicePns: deviceResponse.devicePns },
          );
        });

        it('will commit a value of `true` to `SET_REGISTRATION`', () => {
          expect(commit).toBeCalledWith(SET_REGISTRATION, true);
        });
      });

      describe('not found', () => {
        beforeEach(async () => {
          getSuccess = false;
          await actions.authorised({ commit, state }, JSON.stringify(deviceResponse));
        });

        it('will call the `getV1ApiUsersDevices` endpoint', () => {
          expect($http.getV1ApiUsersDevices).toBeCalledWith(
            { devicePns: deviceResponse.devicePns },
          );
        });

        it('will commit a value of `false` to `SET_REGISTRATION`', () => {
          expect(commit).toBeCalledWith(SET_REGISTRATION, false);
        });
      });
    });

    describe('on toggle', () => {
      beforeEach(() => {
        deviceResponse.trigger = 'toggle';
      });

      describe('not registered', () => {
        beforeEach(async () => {
          state.registered = false;
          await actions.authorised({ commit, state }, JSON.stringify(deviceResponse));
        });

        it('will call the `postV1ApiUsersDevices` endpoint', () => {
          expect($http.postV1ApiUsersDevices).toBeCalledWith({
            addDeviceRequest: {
              devicePns: deviceResponse.devicePns,
              deviceType: deviceResponse.deviceType,
            },
          });
        });

        it('will commit a value of `false` to `SET_WAITING`', () => {
          expect(commit).toBeCalledWith(SET_WAITING, false);
        });

        it('will commit a value of `true` to `SET_REGISTRATION`', () => {
          expect(commit).toBeCalledWith(SET_REGISTRATION, true);
        });
      });

      describe('registered', () => {
        beforeEach(async () => {
          state.registered = true;
          await actions.authorised({ commit, state }, JSON.stringify(deviceResponse));
        });

        it('will call the `deleteV1ApiUsersDevices` endpoint', () => {
          expect($http.deleteV1ApiUsersDevices).toBeCalledWith(
            { devicePns: deviceResponse.devicePns },
          );
        });

        it('will commit a value of `false` to `SET_WAITING`', () => {
          expect(commit).toBeCalledWith(SET_WAITING, false);
        });

        it('will commit a value of `false` to `SET_REGISTRATION`', () => {
          expect(commit).toBeCalledWith(SET_REGISTRATION, false);
        });
      });
    });
  });

  describe('settingsEnabled', () => {
    describe('is enabled', () => {
      beforeEach(() => {
        actions.settingsEnabled({ commit }, true);
      });

      it('will commit a value of `true` to `SET_SETTINGS_ENABLED`', () => {
        expect(commit).toBeCalledWith(SET_SETTINGS_ENABLED, true);
      });

      it('will call native app `requestPnsToken`', () => {
        expect(global.nativeApp.requestPnsToken).toBeCalledWith('load');
      });
    });

    describe('is disabled', () => {
      beforeEach(() => {
        actions.settingsEnabled({ commit }, false);
      });

      it('will commit a value of `false` to `SET_SETTINGS_ENABLED`', () => {
        expect(commit).toBeCalledWith(SET_SETTINGS_ENABLED, false);
      });

      it('will not call native app `requestPnsToken`', () => {
        expect(global.nativeApp.requestPnsToken).not.toBeCalled();
      });

      it('will commit a value of `false` to `SET_REGISTRATION`', () => {
        expect(commit).toBeCalledWith(SET_REGISTRATION, false);
      });
    });
  });

  describe('load', () => {
    let result;

    beforeEach(() => {
      result = actions.load();
    });

    it('will call native app `areNotificationsEnabled`', () => {
      expect(global.nativeApp.areNotificationsEnabled).toBeCalledWith();
    });

    it('will return a promise', () => {
      expect(result).toBeInstanceOf(Promise);
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
      expect(global.nativeApp.requestPnsToken).toBeCalledWith('toggle');
    });
  });

  describe('unauthorised', () => {
    beforeEach(() => {
      actions.unauthorised({ commit });
    });

    it('will commit a value of `false` to `SET_WAITING`', () => {
      expect(commit).toBeCalledWith(SET_WAITING, false);
    });

    it('will commit a value of `false` to `SET_REGISTRATION`', () => {
      expect(commit).toBeCalledWith(SET_REGISTRATION, false);
    });
  });
});
