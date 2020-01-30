import showShutterPage from '@/lib/proxy/shutter';
import { APPOINTMENTS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import { createStore } from '../../helpers';

jest.mock('@/lib/utils');

describe('shutter', () => {
  let $store;
  let self;

  beforeEach(() => {
    jest.clearAllMocks();
    $store = createStore({
      state: {
        errors: {
          apiErrors: [],
        },
        myAppointments: {
          error: null,
        },
      },
    });
    self = { $store };
  });

  describe('proxying', () => {
    beforeEach(() => {
      $store.getters['session/isProxying'] = true;
    });

    describe('shutter page defined', () => {
      let route;

      beforeEach(() => {
        route = {
          proxyShutterPath: '/shutter/example',
        };
      });

      describe('appointments route', () => {
        beforeEach(() => {
          route.name = APPOINTMENTS.name;
        });

        describe('has 403 error', () => {
          beforeEach(() => {
            $store.state.myAppointments.error = { status: 403 };
            showShutterPage(route, self);
          });

          it('will redirect', () => {
            expect(redirectTo).toBeCalledWith(self, '/shutter/example');
          });
        });

        describe('has any other error', () => {
          beforeEach(() => {
            $store.state.myAppointments.error = { status: 500 };
            showShutterPage(route, self);
          });

          it('will not redirect', () => {
            expect(redirectTo).not.toBeCalled();
          });
        });

        describe('has no error', () => {
          beforeEach(() => {
            $store.state.myAppointments.error = null;
          });

          it('will not redirect', () => {
            expect(redirectTo).not.toBeCalled();
          });
        });
      });

      describe('any other route', () => {
        beforeEach(() => {
          route.name = 'foo';
        });

        describe('has 403 api error', () => {
          beforeEach(() => {
            $store.getters['errors/showApiError'] = true;
            $store.state.errors.apiErrors.push({ status: 403 });
            showShutterPage(route, self);
          });

          it('will redirect', () => {
            expect(redirectTo).toBeCalledWith(self, '/shutter/example');
          });
        });

        describe('has any other api error', () => {
          beforeEach(() => {
            $store.getters['errors/showApiError'] = true;
            $store.state.errors.apiErrors.push({ status: 500 });
            showShutterPage(route, self);
          });

          it('will not redirect', () => {
            expect(redirectTo).not.toBeCalled();
          });
        });

        describe('has no api error', () => {
          beforeEach(() => {
            $store.getters['errors/showApiError'] = false;
            $store.state.errors.apiErrors = [];
            showShutterPage(route, self);
          });

          it('will not redirect', () => {
            expect(redirectTo).not.toBeCalled();
          });
        });
      });
    });

    describe('shutter page not defined', () => {
      let route;

      beforeEach(() => {
        route = {
          proxyShutterPath: undefined,
        };
        $store.getters['errors/showApiError'] = true;
        $store.state.errors.apiErrors.push({ status: 403 });
        route.name = APPOINTMENTS.name;
        $store.state.myAppointments.error = { status: 403 };
        showShutterPage(route, self);
      });

      it('will not redirect', () => {
        expect(redirectTo).not.toBeCalled();
      });
    });
  });

  describe('not proxying', () => {
    let route;

    beforeEach(() => {
      $store.getters['session/isProxying'] = false;

      route = {
        name: APPOINTMENTS.name,
        proxyShutterPath: '/shutter/example',
      };
      $store.getters['errors/showApiError'] = true;
      $store.state.errors.apiErrors.push({ status: 403 });
      $store.state.myAppointments.error = { status: 403 };
      showShutterPage(route, self);
    });

    it('will not redirect', () => {
      expect(redirectTo).not.toBeCalled();
    });
  });
});
