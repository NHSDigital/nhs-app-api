import Redirector from '@/pages/redirector/index';
import {
  INTERSTITIAL_REDIRECTOR_NAME,
  REDIRECT_PARAMETER,
  REDIRECT_PAGE_PARAMETER,
  APPOINTMENTS_NAME,
} from '@/router/names';
import { AppPage } from '@/static/js/v1/src/constants';
import { GP_MEDICAL_RECORD_PATH, INDEX_PATH } from '@/router/paths';
import hasAgreedToThirdPartyWarning from '@/lib/sessionStorage';
import * as dependency from '@/lib/utils';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/lib/sessionStorage');

describe('redirector page', () => {
  let $route;
  let $router;
  let wrapper;
  dependency.redirectTo = jest.fn();
  dependency.redirectByName = jest.fn();

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
    wrapper = undefined;
  });

  describe('has no redirect param', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: {},
      };

      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has empty redirect param', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: '' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param to internal route name', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: APPOINTMENTS_NAME },
      };
      wrapper = mountRedirector($http, $store);
    });

    it('will call redirectByName passing redirect param', () => {
      expect(dependency.redirectByName).toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS_NAME);
    });
  });

  describe('has redirect param to internal route path', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(true);

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: GP_MEDICAL_RECORD_PATH },
      };
      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to param value', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, GP_MEDICAL_RECORD_PATH);
    });
  });

  describe('has redirect param to a mis-spelled internal route path', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(false);

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: `${GP_MEDICAL_RECORD_PATH}xyz` },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param external site not on knownServices list', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(false);

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.google.com' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param external site on knownServices list for non third party', () => {
    let $http;

    beforeEach(() => {
      $http = createHttp();
      const $state = {
        knownServices: {
          knownServices: [{
            id: 'nonethirdParty',
            requiresAssertedLoginIdentity: true,
            showThirdPartyWarning: false,
            url: 'http://www.url.com',
          }],
        },
      };
      const $store = createStore({ $http, state: $state });
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(false);
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });
  });

  describe('has redirect param external site on knownServices for pkb and path included in third-party-provider locale', () => {
    let $http;

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
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(false);
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('warning section should be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(true);
    });

    it('will call `postV1PatientAssertedLoginIdentity` on button click', () => {
      const expectedRequest = {
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
          JumpOffId: 'messages',
          ProviderId: 'pkb',
          ProviderName: 'Patients Know Best',
        },
        ignoreError: true,
      };
      const continueButton = wrapper.find('a.nhsuk-button');
      continueButton.trigger('click');

      expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith(expectedRequest);
    });
  });

  describe('has redirect param external site on knownServices for pkb and path not included in third-party-provider locale', () => {
    let $http;

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
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(false);
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/abc?def=ghi' },
      };

      wrapper = mountRedirector($http, $store);
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param not a url or internal path', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(false);

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'something else' },
      };
      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param external site on knownServices for pkb and path included in third-party-provider locale and thirdparty warning has been accepted', () => {
    let $http;

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
      $store.app.isNhsAppPath = jest.fn().mockReturnValue(false);
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
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

    it('will generate an asserted login identity for the matching service', () => {
      const expectedRequest = {
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
          JumpOffId: 'messages',
          ProviderId: 'pkb',
          ProviderName: 'Patients Know Best',
        },
        ignoreError: true,
      };

      expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith(expectedRequest);
    });

    it('does not display the continue button', () => {
      const continueButton = wrapper.contains('a.nhsuk-button');
      expect(continueButton).toEqual(false);
    });
  });

  describe('has redirect to page param', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PAGE_PARAMETER]: AppPage.APPOINTMENTS },
      };
      wrapper = mountRedirector($http, $store);
    });

    it('will call redirectByName passing redirect to page param', () => {
      expect(dependency.redirectByName).toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS_NAME);
    });
  });

  describe('has redirect to page param with invalid value', () => {
    beforeEach(() => {
      const $http = createHttp();
      const $store = createStore({ state: createState() });

      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PAGE_PARAMETER]: 'foo' },
      };
      wrapper = mountRedirector($http, $store);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });
});
