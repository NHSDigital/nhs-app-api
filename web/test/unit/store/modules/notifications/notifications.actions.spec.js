import actions from '@/store/modules/notifications/actions';
import { SET_REGISTRATION, SET_WAITING } from '@/store/modules/notifications/mutation-types';

describe('notifications actions', () => {
  let $http;
  let commit;
  let deleteSuccess;
  let error;
  let getSuccess;
  let postSuccess;

  const promiseReturn = (success) => {
    if (success) {
      return Promise.resolve();
    }
    return Promise.reject(error);
  };

  const toggleOnError = ({ deviceResponse, state }) => {
    let exception;
    const execute = async () => {
      try {
        await actions.authorised({ commit, state }, JSON.stringify(deviceResponse));
      } catch (e) {
        exception = e;
      }
    };

    describe('404', () => {
      beforeEach(async () => {
        error.response.status = 404;
        await execute();
      });

      it('will throw an error', () => {
        expect(exception.message).toBe(error.message);
      });

      it('will dispatch `errors/addApiError` with error code `10003`', () => {
        expect(actions.dispatch).toBeCalledWith('errors/addApiError', {
          message: error.message,
          response: {
            data: {
              errorCode: 10003,
            },
            status: 500,
          },
        });
      });
    });

    describe('any other', () => {
      beforeEach(async () => {
        await execute();
      });

      it('will throw an error', () => {
        expect(exception.message).toBe(error.message);
      });

      it('will not dispatch `errors/addApiError`', () => {
        expect(actions.dispatch).not.toBeCalled();
      });
    });
  };

  beforeEach(() => {
    error = {
      response: {
        status: 500,
      },
      message: 'Error message',
    };
    getSuccess = true;
    deleteSuccess = true;
    postSuccess = true;
    commit = jest.fn();
    $http = {
      deleteV1ApiUsersDevices: jest.fn().mockImplementation(() => promiseReturn(deleteSuccess)),
      getV1ApiUsersDevices: jest.fn().mockImplementation(() => promiseReturn(getSuccess)),
      postV1ApiUsersDevices: jest.fn().mockImplementation(() => promiseReturn(postSuccess)),
    };
    actions.app = {
      get $http() {
        return $http;
      },
    };
    actions.dispatch = jest.fn();
    global.nativeApp = {
      getNotificationsStatus: jest.fn(),
      requestPnsToken: jest.fn(),
    };
  });

  describe('authorised', () => {
    const state = { registered: false };
    const deviceResponse = { deviceType: 'android', devicePns: 5, trigger: 'toggle' };

    describe('on load', () => {
      let loading;

      beforeEach(() => {
        loading = actions.load();
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

        it('will resolve loading promise with `authorised`', () => expect(loading).resolves.toBe('authorised'));
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

        it('will resolve loading promise with `authorised`', () => expect(loading).resolves.toBe('authorised'));
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

        describe('on error', () => {
          beforeEach(() => {
            postSuccess = false;
          });

          toggleOnError({ deviceResponse, state });
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

        describe('on error', () => {
          beforeEach(() => {
            deleteSuccess = false;
          });

          toggleOnError({ deviceResponse, state });
        });
      });
    });
  });

  describe('settingsStatus', () => {
    let loading;

    beforeEach(() => {
      loading = actions.load();
    });

    describe('authorised', () => {
      beforeEach(() => {
        actions.settingsStatus({ commit }, 'authorised');
      });

      it('will call native app `requestPnsToken`', () => {
        expect(global.nativeApp.requestPnsToken).toBeCalledWith('load');
      });
    });

    describe('notDetermined', () => {
      beforeEach(() => {
        actions.settingsStatus({ commit }, 'notDetermined');
      });

      it('will not call native app `requestPnsToken`', () => {
        expect(global.nativeApp.requestPnsToken).not.toBeCalled();
      });

      it('will commit a value of `false` to `SET_REGISTRATION`', () => {
        expect(commit).toBeCalledWith(SET_REGISTRATION, false);
      });

      it('will resolve loading promise with `notDetermined`', () => expect(loading).resolves.toBe('notDetermined'));
    });
  });

  describe('load', () => {
    let result;

    beforeEach(() => {
      result = actions.load();
    });

    it('will call native app `getNotificationsStatus`', () => {
      expect(global.nativeApp.getNotificationsStatus).toBeCalled();
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

    it('will dispatch `errors/addApiError` with error code `10002`', () => {
      expect(actions.dispatch).toBeCalledWith('errors/addApiError', {
        response: {
          status: 500,
          data: {
            errorCode: 10002,
          },
        },
      });
    });
  });
});
