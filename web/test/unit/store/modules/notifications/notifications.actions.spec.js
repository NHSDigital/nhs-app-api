import actions from '@/store/modules/notifications/actions';
import { SET_REGISTRATION, SET_WAITING } from '@/store/modules/notifications/mutation-types';
import { ACCOUNT_NOTIFICATIONS_NAME, NOTIFICATIONS_NAME } from '@/router/names';
import { createRouter } from '../../../helpers';

describe('notifications actions', () => {
  let $http;
  let $router;
  let commit;
  let deleteSuccess;
  let error;
  let getSuccess;
  let postSuccess;
  let logSuccess;

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
        await actions.authorised({ commit, state }, deviceResponse);
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
    logSuccess = true;
    commit = jest.fn();
    $http = {
      deleteV1ApiUsersMeDevices: jest.fn().mockImplementation(() => promiseReturn(deleteSuccess)),
      getV1ApiUsersMeDevices: jest.fn().mockImplementation(() => promiseReturn(getSuccess)),
      postV1ApiUsersMeDevices: jest.fn().mockImplementation(() => promiseReturn(postSuccess)),
      postV1ApiUsersMeDevicesPromptMetrics: jest.fn().mockImplementation(
        () => promiseReturn(logSuccess),
      ),
    };
    $router = createRouter(ACCOUNT_NOTIFICATIONS_NAME);
    $router.history = {
      pending: {
        name: ACCOUNT_NOTIFICATIONS_NAME,
      },
    };
    actions.app = { $http, $router };
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

      describe('Accounts notifications', () => {
        describe('found', () => {
          beforeEach(async () => {
            actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
            getSuccess = true;
            await actions.authorised({ commit, state }, deviceResponse);
          });

          it('will call the `getV1ApiUsersMeDevices` endpoint', () => {
            expect($http.getV1ApiUsersMeDevices).toBeCalledWith(
              { devicePns: deviceResponse.devicePns },
            );
          });

          it('will commit a value of `true` to `SET_REGISTRATION`', () => {
            expect(commit).toBeCalledWith(SET_REGISTRATION, true);
          });

          it('will resolve loading promise with `authorised`', () => expect(loading).resolves.toBe('authorised'));
        });
      });

      describe('not found', () => {
        describe('account notifications', () => {
          beforeEach(async () => {
            actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
            getSuccess = false;
            await actions.authorised({ commit, state }, deviceResponse);
          });

          it('will call the `getV1ApiUsersMeDevices` endpoint', () => {
            expect($http.getV1ApiUsersMeDevices).toBeCalledWith(
              { devicePns: deviceResponse.devicePns },
            );
          });

          it('will commit a value of `false` to `SET_REGISTRATION`', () => {
            expect(commit).toBeCalledWith(SET_REGISTRATION, false);
          });

          it('will resolve loading promise with `authorised`', () => expect(loading).resolves.toBe('authorised'));
        });
      });
    });

    describe('on toggle', () => {
      beforeEach(() => {
        deviceResponse.trigger = 'toggle';
      });

      describe('not registered', () => {
        describe('account notifications', () => {
          beforeEach(async () => {
            state.registered = false;
            actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
            await actions.authorised({ commit, state }, deviceResponse);
          });

          it('will call the `postV1ApiUsersMeDevices` endpoint', () => {
            expect($http.postV1ApiUsersMeDevices).toBeCalledWith({
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
              actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
              postSuccess = false;
            });

            toggleOnError({ deviceResponse, state });
          });
        });
        describe('notifications prompt', () => {
          beforeEach(async () => {
            state.registered = false;
            actions.app.$router.currentRoute.name = NOTIFICATIONS_NAME;
            await actions.authorised({ commit, state }, deviceResponse);
          });

          it('will dispatch to log metrics', () => {
            expect(actions.dispatch).toBeCalledWith('notifications/logMetrics', {
              screenShown: true,
              notificationsRegistered: true,
            });
          });

          describe('on error', () => {
            const execute = async () => {
              await actions.authorised({ commit, state }, deviceResponse);
            };
            beforeEach(() => {
              actions.app.$router.currentRoute.name = NOTIFICATIONS_NAME;
              postSuccess = false;
              execute();
            });

            it('will dispatch to log metrics', () => {
              expect(actions.dispatch).toBeCalledWith('notifications/logMetrics', {
                screenShown: true,
                notificationsRegistered: true,
              });
            });
          });
        });
      });

      describe('registered', () => {
        describe('account notifications', () => {
          beforeEach(async () => {
            actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
            state.registered = true;
            await actions.authorised({ commit, state }, deviceResponse);
          });

          describe('on error', () => {
            beforeEach(() => {
              deleteSuccess = false;
            });

            toggleOnError({ deviceResponse, state });
          });
        });
        describe('notifications prompt', () => {
          beforeEach(async () => {
            actions.app.$router.currentRoute.name = NOTIFICATIONS_NAME;
            state.registered = true;
            await actions.authorised({ commit, state }, deviceResponse);
          });

          describe('on error', () => {
            const execute = async () => {
              await actions.authorised({ commit, state }, deviceResponse);
            };
            beforeEach(() => {
              deleteSuccess = false;
              execute();
            });

            it('will dispatch to log metrics', () => {
              expect(actions.dispatch).toBeCalledWith('notifications/logMetrics', {
                screenShown: true,
                notificationsRegistered: false,
              });
            });
          });
        });
      });
    });
  });

  describe('settingsStatus', () => {
    describe('account notifications', () => {
      let loading;

      beforeEach(() => {
        actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
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
    describe('notifications prompt', () => {
      let loading;

      beforeEach(() => {
        actions.app.$router.history.pending.name = NOTIFICATIONS_NAME;
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

      describe('denied', () => {
        beforeEach(() => {
          actions.settingsStatus({ commit }, 'denied');
        });

        it('will push error screen`', () => {
          expect(actions.app.$router.push).toBeCalled();
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
  });

  describe('logMetrics', () => {
    const rootState = {
      device: {
        source: 'android',
      },
    };

    beforeEach(async () => {
      actions.app.$router.currentRoute.name = NOTIFICATIONS_NAME;
      await actions.logMetrics({ rootState },
        { screenShown: true, notificationsRegistered: true });
    });

    describe('postV1ApiUsersMeDevicesPromptMetrics', () => {
      it('will call `postV1ApiUsersMeDevicesPromptMetrics`', () => {
        expect($http.postV1ApiUsersMeDevicesPromptMetrics).toBeCalledWith({
          notificationsPromptData: {
            screenShown: true,
            notificationsRegistered: true,
            platform: 'android',
          },
        });
      });
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
      actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
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
    describe('account notifications', () => {
      beforeEach(() => {
        actions.app.$router.currentRoute.name = ACCOUNT_NOTIFICATIONS_NAME;
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
    describe('notifications prompt', () => {
      beforeEach(() => {
        actions.app.$router.currentRoute.name = NOTIFICATIONS_NAME;
        actions.unauthorised({ commit });
      });

      it('will commit a value of `false` to `SET_WAITING`', () => {
        expect(commit).toBeCalledWith(SET_WAITING, false);
      });

      it('will dispatch to log metrics', () => {
        expect(actions.dispatch).toBeCalledWith('notifications/logMetrics', {
          screenShown: true,
          notificationsRegistered: false,
        });
      });
    });
  });
});
