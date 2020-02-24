import Redirector from '@/pages/redirector/index';
import { APPOINTMENTS, INDEX, INTERSTITIAL_REDIRECTOR, REDIRECT_PARAMETER } from '@/lib/routes';
import { createRouter, createStore, mount } from '../../helpers';

describe('redirector page', () => {
  let $route;
  let $router;

  const createState = () => ({
    knownServices: {
      knownServices: [{
        requiresAssertedLoginIdentity: false,
        url: '',
      }],
    },
  });

  const createHttp = () => ({
    postV1PatientAssertedLoginIdentity: jest.fn()
      .mockImplementation(() => Promise.resolve({ token: 'jwtToken' })),
  });

  const mountRedirector = ($http, $store) => {
    mount(Redirector, {
      $http,
      $store,
      $router,
      $route,
    });
  };

  beforeEach(() => {
    $router = createRouter();
  });

  describe('has no redirect param', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: {},
      };

      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });

  describe('has redirect param to internal route', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: APPOINTMENTS.name },
      };
      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
    });
  });

  describe('has redirect param external site not on knownServices list', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'http://www.google.com' },
      };

      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });

  describe('has redirect param external site on knownServices list', () => {
    let $http;
    beforeEach(() => {
      $http = createHttp();
      const $state = {
        knownServices: {
          knownServices: [{
            requiresAssertedLoginIdentity: true,
            url: 'www.url.com',
          }],
        },
      };
      const $store = createStore({ $http, state: $state });
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'www.url.com' },
      };

      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      const expectedRequest = { assertedLoginIdentityRequest: { intendedRelyingPartyUrl: 'www.url.com' } };
      expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith(expectedRequest);
    });
  });

  describe('has redirect param not a url or internal path', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'something else' },
      };
      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });
});
