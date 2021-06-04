import * as dependency from '@/lib/utils';
import * as names from '@/router/names';
import * as window from '@/lib/window';
import NativeApp from '@/services/native-app';
import Redirector from '@/pages/redirector/index';
import VueRouter from 'vue-router';
import hasAgreedToThirdPartyWarning from '@/lib/sessionStorage';
import { AppPage } from '@/static/js/v1/src/constants';
import {
  GP_MEDICAL_RECORD_PATH,
  INDEX_PATH,
  SILVER_INTEGRATION_FEATURE_NOT_AVAILABLE_PATH,
  UPLIFT_SILVER_INTEGRATION_PATH,
} from '@/router/paths';
import { allRoutes } from '@/router';
import { createStore, localVue, mount } from '../../helpers';

jest.mock('@/lib/sessionStorage');
jest.mock('@/services/native-app');

describe('redirector page', () => {
  let wrapper;
  let $http;
  let $route;

  const nhsAppBaseUrl = 'https://www.nhsapp.com';

  localVue.use(VueRouter);

  const mountRedirector = ({
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

    const router = new VueRouter({
      mode: 'history',
      base: nhsAppBaseUrl,
      routes: allRoutes,
    });

    router.push($route);

    return mount(Redirector, { $store, router });
  };

  beforeEach(() => {
    dependency.redirectTo = jest.fn();
    dependency.redirectByName = jest.fn();
    window.getWindowLocationOrigin = jest.fn().mockReturnValue(nhsAppBaseUrl);
    $route = {};
    wrapper = undefined;
  });

  describe('has no redirect parameter', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: {},
        meta: {},
      };

      wrapper = mountRedirector();
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has an empty redirect parameter', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: '' },
        meta: {},
      };

      wrapper = mountRedirector();
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has a redirect parameter to an internal route name', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: names.APPOINTMENTS_NAME },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will call redirectByName passing redirect param', () => {
      expect(dependency.redirectByName).toHaveBeenCalledWith(wrapper.vm, names.APPOINTMENTS_NAME);
    });
  });

  describe('has a redirect parameter that is an absolute path to an internal path', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: `${nhsAppBaseUrl}/${GP_MEDICAL_RECORD_PATH}` },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will redirectByName to param value', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, `/patient/${GP_MEDICAL_RECORD_PATH}`);
    });
  });

  describe('has a redirect parameter that is an encoded absolute path to an internal path', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: `https%3A%2F%2Fwww.nhsapp.com%2F${GP_MEDICAL_RECORD_PATH}` },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will redirectByName to param value', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, `/patient/${GP_MEDICAL_RECORD_PATH}`);
    });
  });

  describe('has a redirect parameter to an internal path', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: GP_MEDICAL_RECORD_PATH },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will redirectByName to param value', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, `/patient/${GP_MEDICAL_RECORD_PATH}`);
    });
  });

  describe('has a redirect parameter for an non existent internal path', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: `${GP_MEDICAL_RECORD_PATH}xyz` },
        meta: {},
      };

      wrapper = mountRedirector({ hasKnownService: false });
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has a redirect parameter to an external site not in the knownServices list', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.google.com' },
        meta: {},
      };

      wrapper = mountRedirector({ hasKnownService: false });
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has a redirect parameter to an external site which is in the knownServices list, with no third party configuration', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com' },
        meta: {},
      };

      wrapper = mountRedirector({
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: false,
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

  describe('has a redirect parameter to an external site in the knownServices list and the path is included in the third-party-provider locale on web', () => {
    beforeEach(() => {
      NativeApp.supportsNativeWebIntegration.mockReturnValue(false);
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      window.setWindowLocation = jest.fn();

      wrapper = mountRedirector({
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        isSilverIntegrationEnabled: true,
        isProofLevel9: true,
      });
    });

    it('warning section should be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(true);
    });

    it('will set tab title to be the name of the feature', () => {
      expect(wrapper.vm.$route.meta.titleKey).toBe('Messages and online consultations');
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

      it('will redirect to the external url', () => {
        expect(window.setWindowLocation).toHaveBeenCalledWith('http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages&assertedLoginIdentity=jwtToken');
      });
    });

    afterEach(() => {
      hasAgreedToThirdPartyWarning.mockClear();
    });
  });

  describe('has a redirect parameter to an external P5 jump off', () => {
    beforeEach(() => {
      NativeApp.supportsNativeWebIntegration.mockReturnValue(false);
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com/covid-status-sso?proofLevel=p5' },
        meta: {},
      };

      window.setWindowLocation = jest.fn();
    });

    describe.each([
      ['P9 user', true],
      ['P5 user', false],
    ])('%s', (_, isProofLevel9) => {
      beforeEach(() => {
        wrapper = mountRedirector({
          hasKnownService: true,
          knownServiceId: 'netCompany',
          requiresAssertedLoginIdentity: true,
          showThirdPartyWarning: false,
          isSilverIntegrationEnabled: true,
          isProofLevel9,
        });
      });

      it('will call `postV1PatientAssertedLoginIdentity`', () => {
        expect($http.postV1PatientAssertedLoginIdentity).toHaveBeenCalledWith({
          assertedLoginIdentityRequest: {
            IntendedRelyingPartyUrl: 'http://www.url.com/covid-status-sso?proofLevel=p5',
            JumpOffId: 'vaccineRecordP5',
            ProviderId: 'netCompany',
            ProviderName: 'the Department of Health and Social Care',
            Action: 'SilverIntegrationJumpOff',
          },
          ignoreError: true,
        });
      });

      it('will redirect to the external url', () => {
        expect(window.setWindowLocation).toHaveBeenCalledWith('http://www.url.com/covid-status-sso?proofLevel=p5&assertedLoginIdentity=jwtToken');
      });
    });

    afterEach(() => {
      hasAgreedToThirdPartyWarning.mockClear();
    });
  });

  describe('has a redirect parameter to an external site in the knownServices list and the path is included in the third-party-provider locale on native app', () => {
    beforeEach(() => {
      NativeApp.supportsNativeWebIntegration.mockReturnValue(true);
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http%3A%2F%2Fwww.url.com%2Fnhs-login%2Flogin?phrPath=%2Fauth%2FgetInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
        hasKnownService: true,
        requiresAssertedLoginIdentity: true,
        showThirdPartyWarning: true,
        isSilverIntegrationEnabled: true,
        isProofLevel9: true,
      });
    });

    it('warning section should be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(true);
    });

    it('will set tab title to be the name of the feature', () => {
      expect(wrapper.vm.$route.meta.titleKey).toBe('Messages and online consultations');
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

      it('will call NativeApp.openWebIntegration with the decoded asserted login url', () => {
        expect(NativeApp.openWebIntegration).toHaveBeenCalledWith('http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages&assertedLoginIdentity=jwtToken');
      });
    });

    afterEach(() => {
      hasAgreedToThirdPartyWarning.mockClear();
    });
  });

  describe('has a redirect parameter to an external site in the knownServices list, and the path is not included in the third-party-provider locale', () => {
    beforeEach(() => {
      hasAgreedToThirdPartyWarning.mockReturnValue(false);
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com/abc?def=ghi' },
        meta: {},
      };

      wrapper = mountRedirector({
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

  describe('has a redirect parameter which is not a url or internal path', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'something else' },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has a redirect parameter to an external site in the knownServices list with the SJR feature enabled but not P9 proof level', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
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

  describe('has a redirect parameter to an external site in the knownServices list with the SJR feature disabled but not P9 proof level', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
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

  describe('has a redirect parameter to an external site in the knownServices list but doesn\'t have the silver integration feature enabled via SJR', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
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

  describe('has a redirect parameter to an external site in the knownServices list and the path is included in third-party-provider locale and the thirdparty warning has been accepted', () => {
    beforeEach(() => {
      hasAgreedToThirdPartyWarning.mockReturnValue(true);
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com/nhs-login/login?phrPath=/auth/getInbox.action?tab=messages' },
        meta: {},
      };

      wrapper = mountRedirector({
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
      expect(wrapper.vm.$route.meta.titleKey).toBe('Messages and online consultations');
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

  describe('has redirect to page parameter set to a valid page enum', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PAGE_PARAMETER]: AppPage.APPOINTMENTS },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will call redirectByName passing redirect to page param', () => {
      expect(dependency.redirectByName).toHaveBeenCalledWith(wrapper.vm, names.APPOINTMENTS_NAME);
    });
  });

  describe('has redirect to page parameter set to a invalid value', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PAGE_PARAMETER]: 'foo' },
        meta: {},
      };
      wrapper = mountRedirector();
    });

    it('will redirect to INDEX path', () => {
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('has a redirect parameter to an external site in the knownServices list which is not a silver integration', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: 'http://www.url.com' },
        meta: {},
      };

      NativeApp.supportsNativeWebIntegration.mockReturnValue(false);
      window.setWindowLocation = jest.fn();

      wrapper = mountRedirector({
        hasKnownService: true,
        knownServiceId: 'unknown',
        requiresAssertedLoginIdentity: false,
        showThirdPartyWarning: false,
      });
    });

    it('will redirect to the external url', () => {
      expect(window.setWindowLocation).toHaveBeenCalledWith('http://www.url.com');
    });

    it('warning section should not be shown', () => {
      expect(wrapper.vm.shouldShowWarning).toEqual(false);
    });
  });

  describe('has a redirect parameter to an encoded unknown absolute path', () => {
    beforeEach(() => {
      $route = {
        name: names.INTERSTITIAL_REDIRECTOR_NAME,
        query: { [names.REDIRECT_PARAMETER]: `${nhsAppBaseUrl}/aptt` },
        meta: {},
      };
      wrapper = mountRedirector();
    });
    it('will redirectByName to param value', () => {
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });
});
