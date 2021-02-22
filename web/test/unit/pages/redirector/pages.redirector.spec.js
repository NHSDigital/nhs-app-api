import Redirector from '@/pages/redirector/index';
import {
  INTERSTITIAL_REDIRECTOR_NAME,
  REDIRECT_PARAMETER,
  REDIRECT_PAGE_PARAMETER,
  APPOINTMENTS_NAME,
} from '@/router/names';
import { AppPage } from '@/static/js/v1/src/constants';
import {
  GP_MEDICAL_RECORD_PATH,
  INDEX_PATH,
  UPLIFT_SILVER_INTEGRATION_PATH,
  SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
} from '@/router/paths';
import hasAgreedToThirdPartyWarning from '@/lib/sessionStorage';
import * as dependency from '@/lib/utils';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/lib/sessionStorage');
jest.mock('@/services/native-app');

describe('redirector page', () => {
  let $route;
  let $router;
  let wrapper;
  let $http;

  const mountRedirector = ({
    isNhsAppPath = true,
    hasKnownService = false,
    knownServiceId = 'pkb',
    requiresAssertedLoginIdentity = false,
    showThirdPartyWarning = false,
    isSilverIntegrationEnabled = false,
    isProofLevel9 = false,
  } = {}) => {
    $http = {
      postV1PatientAssertedLoginIdentity: jest.fn()
        .mockImplementation(() => Promise.resolve({ token: 'jwtToken' })),
    };

    const $store = createStore({
      $http,
      getters: {
        'knownServices/matchOneByUrl': jest.fn().mockImplementation((url) => {
          if (hasKnownService) {
            return {
              id: knownServiceId,
              requiresAssertedLoginIdentity,
              showThirdPartyWarning,
              url,
            };
          }
          return undefined;
        }),
        'session/isProofLevel9': isProofLevel9,
        'serviceJourneyRules/silverIntegrationEnabled': jest.fn().mockImplementation(() => isSilverIntegrationEnabled),
      },
    });

    $store.app.isNhsAppPath = jest.fn().mockReturnValue(isNhsAppPath);

    return mount(Redirector, { $store, $router, $route });
  };

  beforeEach(() => {
    dependency.redirectTo = jest.fn();
    dependency.redirectByName = jest.fn();
    $router = createRouter();
    $route = {};
    wrapper = undefined;
  });

  describe('has no redirect param', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: {},
        meta: {},
      };

      wrapper = mountRedirector();
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has empty redirect param', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: '' },
        meta: {},
      };

      wrapper = mountRedirector();
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param to internal route name', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: APPOINTMENTS_NAME },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will call redirectByName passing redirect param', () => {
      expect(dependency.redirectByName).toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS_NAME);
    });
  });

  describe('has redirect param to internal route path', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: GP_MEDICAL_RECORD_PATH },
        meta: {},
      };
      wrapper = mountRedirector({ isNhsAppPath: true });
    });

    it('will redirect to param value', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, GP_MEDICAL_RECORD_PATH);
    });
  });

  describe('has redirect param to a mis-spelled internal route path', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: `${GP_MEDICAL_RECORD_PATH}xyz` },
        meta: {},
      };

      wrapper = mountRedirector({ hasKnownService: false, isNhsAppPath: false });
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param external site not on knownServices list', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.google.com' },
        meta: {},
      };

      wrapper = mountRedirector({ hasKnownService: false, isNhsAppPath: false });
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param external site on knownServices list for unknown third party', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com' },
        meta: {},
      };

      wrapper = mountRedirector({
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: false,
        isNhsAppPath: false,
        isSilverIntegrationEnabled: true,
      });
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });
  });

  describe('has redirect param external site on knownServices list for an unknown third party', () => {
    const { location } = global;

    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com' },
        meta: {},
      };

      delete global.location;

      wrapper = mountRedirector({
        isNhsAppPath: false,
        hasKnownService: true,
        knownServiceId: 'unknown',
        requiresAssertedLoginIdentity: false,
        showThirdPartyWarning: false,
        isSilverIntegrationEnabled: true,
      });
    });

    it('will redirect using location to external url', () => {
      expect(global.location).toBe('http://www.url.com');
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });

    afterEach(() => {
      global.location = location;
    });
  });


  describe('has redirect param external site on knownServices for pkb and path included in third-party-provider locale', () => {
    beforeEach(() => {
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        isNhsAppPath: false,
        isSilverIntegrationEnabled: true,
        isProofLevel9: true,
      });
    });

    it('warning section should be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(true);
    });

    it('will set tab title to be the name of the feature', () => {
      expect($route.meta.titleKey).toBe('Messages and online consultations');
    });

    describe('on continue button click', () => {
      beforeEach(() => {
        const continueButton = wrapper.find('a.nhsuk-button');
        continueButton.trigger('click');
      });

      it('will call `postV1PatientAssertedLoginIdentity`', () => {
        expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
            JumpOffId: 'messages',
            ProviderId: 'pkb',
            ProviderName: 'Patients Know Best',
            Action: 'SilverIntegrationJumpOff',
          },
          ignoreError: true,
        });
      });
    });

    afterEach(() => {
      hasAgreedToThirdPartyWarning.mockClear();
    });
  });

  describe('has redirect param external site on knownServices for pkb and path not included in third-party-provider locale', () => {
    beforeEach(() => {
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/abc?def=ghi' },
        meta: {},
      };

      wrapper = mountRedirector({
        isNhsAppPath: false,
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
      });
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });

    afterEach(() => {
      hasAgreedToThirdPartyWarning.mockClear();
    });
  });

  describe('has redirect param not a url or internal path', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'something else' },
        meta: {},
      };
      wrapper = mountRedirector({ isNhsAppPath: false });
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has redirect param external site on knownServices for pkb with SJR feature enabled but is not P9 proof level', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
        isNhsAppPath: false,
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        isSilverIntegrationEnabled: true,
        isProofLevel9: false,
      });
    });

    it('will redirect to UPLIFT_SILVER_INTEGRATION_PATH path', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, UPLIFT_SILVER_INTEGRATION_PATH, expect.any(Object));
    });
  });

  describe('has redirect param external site on knownServices for pkb with SJR feature disabled but is not P9 proof level', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
        isNhsAppPath: false,
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        isSilverIntegrationEnabled: false,
        isProofLevel9: false,
      });
    });

    it('will redirect to UPLIFT_SILVER_INTEGRATION_PATH path', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, UPLIFT_SILVER_INTEGRATION_PATH, expect.any(Object));
    });
  });

  describe('has redirect param external site on knownServices but doesn\'t have silver integration feature enabled via SJR', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
        isNhsAppPath: false,
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        isProofLevel9: true,
        isSilverIntegrationEnabled: false,
      });
    });

    it('will redirect to SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH path', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(
          wrapper.vm,
          SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
          expect.any(Object),
        );
    });
  });

  describe('has redirect param external site on knownServices for pkb and path included in third-party-provider locale and thirdparty warning has been accepted', () => {
    beforeEach(() => {
      hasAgreedToThirdPartyWarning.mockReturnValue(true);
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
        isNhsAppPath: false,
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        isProofLevel9: true,
        isSilverIntegrationEnabled: true,
      });
    });

    it('warning section should be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });

    it('session storage should be correctly named', () => {
      expect(wrapper.vm.sessionStorageName).toEqual('agreedThirdPartyWarning_pkb');
    });

    it('will set tab title to be the name of the feature', () => {
      expect($route.meta.titleKey).toBe('Messages and online consultations');
    });

    it('will generate an asserted login identity for the matching service', () => {
      expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith({
        assertedLoginIdentityRequest: {
          IntendedRelyingPartyUrl: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages',
          JumpOffId: 'messages',
          ProviderId: 'pkb',
          ProviderName: 'Patients Know Best',
          Action: 'SilverIntegrationJumpOff',
        },
        ignoreError: true,
      });
    });

    it('does not display the continue button', () => {
      const continueButton = wrapper.contains('a.nhsuk-button');
      expect(continueButton).toEqual(false);
    });

    afterEach(() => {
      hasAgreedToThirdPartyWarning.mockClear();
    });
  });

  describe('has redirect to page param', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PAGE_PARAMETER]: AppPage.APPOINTMENTS },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will call redirectByName passing redirect to page param', () => {
      expect(dependency.redirectByName).toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS_NAME);
    });
  });

  describe('has redirect to page param with invalid value', () => {
    beforeEach(() => {
      $route = {
        name: INTERSTITIAL_REDIRECTOR_NAME,
        query: { [REDIRECT_PAGE_PARAMETER]: 'foo' },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });
});
