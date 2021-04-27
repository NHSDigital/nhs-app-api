import each from 'jest-each';
import mockdate from 'mockdate';
import moment from 'moment';
import {
  createConditionalRedirectRouteByName,
  redirectTo,
  redirectByName,
} from '@/lib/utils';
import { INDEX_PATH, INDEX_PATH_PARAM, NOTIFICATIONS_PATH } from '@/router/paths';
import { INTERSTITIAL_REDIRECTOR_NAME, MESSAGES_NAME, REDIRECT_PARAMETER } from '@/router/names';

jest.mock('@/services/native-app');
jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

let self;

describe('util library redirect', () => {
  let loggedIn = false;
  const PATIENT_ID = 1234;

  beforeEach(() => {
    self = {
      $store: {
        app: {
          context: {
            redirect: jest.fn(),
          },
        },
        state: {
          device: {
            isNativeApp: false,
            source: 'web',
          },
        },
        getters: {
          'session/isLoggedIn': () => loggedIn,
          'linkedAccounts/getPatientId': PATIENT_ID,
          'linkedAccounts/isPatientIdNotEmpty': true,
        },
      },
      $router: {
        currentRoute: {
          path: 'foo',
          params: {},
          name: 'bar',
        },
        go: jest.fn(),
        push: jest.fn(),
      },
    };
  });

  describe('createConditionalRedirectRouteByName', () => {
    const name = 'foo';
    const store = {
      getters: {
        'session/isLoggedIn': jest.fn(),
      },
    };

    describe('has no redirect parameter', () => {
      let route;

      beforeEach(() => {
        route = createConditionalRedirectRouteByName({ name, query: {}, store });
      });

      it('will retain route name', () => {
        expect(route).toStrictEqual({ name, query: {}, params: {} });
      });
    });

    describe('has redirect parameter to route name', () => {
      let route;

      beforeEach(() => {
        const query = {
          [REDIRECT_PARAMETER]: MESSAGES_NAME,
        };
        route = createConditionalRedirectRouteByName({ name, query, store });
      });

      it('will replace route name with redirect parameter', () => {
        expect(route).toStrictEqual({ name: MESSAGES_NAME, query: {}, params: {} });
      });
    });

    describe.each([
      NOTIFICATIONS_PATH,
      'https://www.example.com',
    ])('has redirect parameter to `%s` path', (path) => {
      const query = {
        [REDIRECT_PARAMETER]: path,
      };
      let route;

      beforeEach(() => {
        route = createConditionalRedirectRouteByName({ name, query, store });
      });

      it('will replace route name with interstitial route name and pass redirect parameter', () => {
        expect(route).toStrictEqual({ name: INTERSTITIAL_REDIRECTOR_NAME, query, params: {} });
      });
    });
  });

  describe('redirectTo', () => {
    describe('logged out', () => {
      beforeEach(() => {
        loggedIn = false;
      });

      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: 'a-path', query });
      });

      each([
        null,
        undefined,
      ]).it('will call push with path only if query value is `%s`', (query) => {
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: 'a-path' });
      });

      describe('same page', () => {
        let path;
        let ts;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ path } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectTo(self, path);
          expect(self.$router.push).toBeCalledWith({ path, query: { ts } });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({
            path,
            query: {
              ...query,
              ts,
            },
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({ path, query });
        });
      });
    });

    describe('logged in', () => {
      const completePath = INDEX_PATH.replace(INDEX_PATH_PARAM, PATIENT_ID);

      beforeEach(() => {
        loggedIn = true;
      });
      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: `${completePath}a-path`, query });
      });

      each([
        null,
        undefined,
      ]).it('will call push with path only if query value is `%s`', (query) => {
        redirectTo(self, 'a-path', query);
        expect(self.$router.push).toBeCalledWith({ path: `${completePath}a-path` });
      });

      describe('same page', () => {
        let path;
        let ts;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ path } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectTo(self, path);
          expect(self.$router.push).toBeCalledWith({ path: completePath + path, query: { ts } });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({
            path: completePath + path,
            query: {
              ...query,
              ts,
            },
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectTo(self, path, query);
          expect(self.$router.push).toBeCalledWith({ path: completePath + path, query });
        });
      });
    });
  });

  describe('redirectByName', () => {
    describe('logged out', () => {
      const params = {};

      beforeEach(() => {
        loggedIn = false;
      });

      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        redirectByName(self, 'a-name', query);
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', query, params });
      });

      each([
        null,
        undefined,
      ]).it('will call push with name only if query value is `%s`', (query) => {
        redirectByName(self, 'a-name', query);
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', params });
      });

      describe('same page', () => {
        let name;
        let ts;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ name } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectByName(self, name);
          expect(self.$router.push).toBeCalledWith({ name, query: { ts }, params });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({
            name,
            query: {
              ...query,
              ts,
            },
            params,
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({ name, query, params });
        });
      });
    });

    describe('logged in', () => {
      beforeEach(() => {
        loggedIn = true;
      });
      it('will call push with query if passed', () => {
        const query = { source: 'ios' };
        const params = { patientId: PATIENT_ID };
        redirectByName(self, 'a-name', query);
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', query, params });
      });

      each([
        null,
        undefined,
      ]).it('will call push with path only if query value is `%s`', (query) => {
        redirectByName(self, 'a-name', query);
        const params = { patientId: PATIENT_ID };
        expect(self.$router.push).toBeCalledWith({ name: 'a-name', params });
      });

      describe('multiple parameters', () => {
        beforeEach(() => {
          loggedIn = true;
          self.$router.currentRoute.params = { pearl: 'jam' };
        });

        it('will call push with param if passed', () => {
          const query = { source: 'ios' };
          redirectByName(self, 'a-name', query);
          expect(self.$router.push).toBeCalledWith({
            name: 'a-name',
            query,
            params: {
              ...self.$router.currentRoute.params,
              patientId: PATIENT_ID,
            },
          });
        });
      });

      describe('no patient Id', () => {
        beforeEach(() => {
          loggedIn = true;
          self.$store.getters['linkedAccounts/isPatientIdNotEmpty'] = false;
        });
        it('will call push with no patientId param', () => {
          const query = { source: 'ios' };
          redirectByName(self, 'a-name', query);
          expect(self.$router.push).toBeCalledWith({ name: 'a-name', query, params: {} });
        });
      });

      describe('same page', () => {
        let ts;
        let name;

        beforeEach(() => {
          const nowDate = moment();
          mockdate.set(nowDate);
          ts = nowDate.unix();

          ({ name } = self.$router.currentRoute);
        });

        afterEach(() => {
          mockdate.reset();
        });

        it('will call push with timespan if query is not passed', () => {
          redirectByName(self, name);
          expect(self.$router.push).toBeCalledWith({
            name,
            query: { ts },
            params: { patientId: PATIENT_ID },
          });
        });

        it('will call push with passed query and timespan if it is the same as current', () => {
          const query = { source: 'ios', ts: 12345678 };
          self.$router.currentRoute.query = query;
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({
            name,
            query: {
              ...query,
              ts,
            },
            params: { patientId: PATIENT_ID },
          });
        });

        it('will call push with passed query if not the same as current', () => {
          const query = { source: 'ios' };
          redirectByName(self, name, query);
          expect(self.$router.push).toBeCalledWith({
            name,
            query,
            params: { patientId: PATIENT_ID },
          });
        });
      });
    });
  });
});
