import Redirector from '@/pages/redirector/index';
import { APPOINTMENTS, INDEX, INTERSTITIAL_REDIRECTOR, REDIRECT_PARAMETER } from '@/lib/routes';
import { createRouter, createStore, mount } from '../../helpers';
import hasAgreedToThirdPartyWarning from '@/lib/sessionStorage';

jest.mock('@/lib/sessionStorage');

describe('redirector page', () => {
  let $route;
  let $router;

  const createState = () => ({
    knownServices: {
      knownServices: [{
        requiresAssertedLoginIdentity: false,
        url: 'http://www.url.com',
      }],
    },
  });

  const createHttp = () => ({
    postV1PatientAssertedLoginIdentity: jest.fn()
      .mockImplementation(() => Promise.resolve({ token: 'jwtToken' })),
  });

  const mountRedirector = ($http, $store) => mount(Redirector, { $http, $store, $router, $route });

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

  describe('has empty redirect param', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: '' },
      };

      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
    });
  });

  describe('has redirect param to internal route name', () => {
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

  describe('has redirect param to internal route path', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: APPOINTMENTS.path },
      };
      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
    });
  });

  describe('has redirect param to a mis-spelled internal route path', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: `${APPOINTMENTS.path}xyz` },
      };

      mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
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

  describe('has redirect param external site on knownServices list for non third party', () => {
    let $http;
    let wrapper;
    beforeEach(() => {
      $http = createHttp();
      const $state = {
        knownServices: {
          knownServices: [{
            id: 'nonethirdParty',
            requiresAssertedLoginIdentity: true,
            showThirdPartyWarning: false,
            url: 'www.url.com',
          }],
        },
      };
      const $store = createStore({ $http, state: $state });
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'www.url.com' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('will call router push with INDEX path', () => {
      const expectedRequest = { assertedLoginIdentityRequest: { IntendedRelyingPartyUrl: 'www.url.com' } };
      expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith(expectedRequest);
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });

    it('text computed properties should be empty strings', () => {
      expect(wrapper.vm.paragraphText()).toEqual('');
      expect(wrapper.vm.linkText()).toEqual('');
      expect(wrapper.vm.providerName()).toEqual('');
    });
  });

  describe('has redirect param external site on knownServices for pkb and path included in third-party-provider locale', () => {
    let $http;
    let wrapper;
    beforeEach(() => {
      $http = createHttp();
      const $state = {
        knownServices: {
          knownServices: [{
            id: 'pkb',
            requiresAssertedLoginIdentity: true,
            showThirdPartyWarning: true,
            url: 'http://www.url.com',
          }],
        },
      };
      hasAgreedToThirdPartyWarning.mockClear();
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      const $store = createStore({ $http, state: $state });
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('warning section should be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(true);
    });

    it('will call router push on button click', () => {
      const expectedRequest = { assertedLoginIdentityRequest: {
        IntendedRelyingPartyUrl: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' } };
      const continueButton = wrapper.find('a.nhsuk-button');
      continueButton.trigger('click');

      expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith(expectedRequest);
    });

    it('text computed properties should not be empty strings', () => {
      expect(wrapper.vm.paragraphText()).toEqual('translate_thirdPartyProviders.warningConjunctions.paragraph');
      expect(wrapper.vm.linkText()).toEqual('translate_thirdPartyProviders.warningConjunctions.linkText');
      expect(wrapper.vm.providerName()).toEqual('translate_thirdPartyProviders.pkb.providerName');
    });
  });

  describe('has redirect param external site on knownServices for pkb and path not included in third-party-provider locale', () => {
    let $http;
    let wrapper;
    beforeEach(() => {
      $http = createHttp();
      const $state = {
        knownServices: {
          knownServices: [{
            id: 'pkb',
            requiresAssertedLoginIdentity: true,
            showThirdPartyWarning: true,
            url: 'http://www.url.com',
          }],
        },
      };
      hasAgreedToThirdPartyWarning.mockClear();
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      const $store = createStore({ $http, state: $state });
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/abc?def=ghi' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });

    it('will call router push with INDEX path', () => {
      expect($router.push).toHaveBeenCalledWith(INDEX.path);
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

  describe('has redirect param external site on knownServices for pkb and path included in third-party-provider locale and thirdparty warning has been accepted', () => {
    let $http;
    let wrapper;
    beforeEach(() => {
      $http = createHttp();
      const $state = {
        knownServices: {
          knownServices: [{
            id: 'pkb',
            requiresAssertedLoginIdentity: true,
            showThirdPartyWarning: true,
            url: 'http://www.url.com',
          }],
        },
      };
      hasAgreedToThirdPartyWarning.mockClear();
      hasAgreedToThirdPartyWarning.mockReturnValue(true);
      const $store = createStore({ $http, state: $state });
      $route = {
        ...INTERSTITIAL_REDIRECTOR,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('warning section should be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });

    it('session storage should be correctly named', () => {
      expect(wrapper.vm.sessionStorageName).toEqual('agreedThirdPartyWarning_pkb');
    });

    it('will call router push', () => {
      const expectedRequest = { assertedLoginIdentityRequest: {
        IntendedRelyingPartyUrl: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' } };

      expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith(expectedRequest);
    });

    it('does not display the continue button', () => {
      const continueButton = wrapper.contains('a.nhsuk-button');
      expect(continueButton).toEqual(false);
    });

    it('text computed properties should not be empty strings', () => {
      expect(wrapper.vm.paragraphText()).toEqual('');
      expect(wrapper.vm.linkText()).toEqual('');
      expect(wrapper.vm.providerName()).toEqual('');
    });
  });
});
