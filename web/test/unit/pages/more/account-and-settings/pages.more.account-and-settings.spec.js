import AccountAndSettingsPage from '@/pages/more/account-and-settings/index';
import i18n from '@/plugins/i18n';
import { createStore, initFilters, mount } from '../../../helpers';

describe('Account and Settings Page', () => {
  jest.mock('@/services/native-app');
  let wrapper;
  initFilters();
  let $state;
  beforeEach(() => {
    $state = {
      device: {
        isNativeApp: false,
        hasBiometricType: true,
      },
      serviceJourneyRules: {
        rules: {
          supportsLinkedProfiles: false,
        },
      },
      session: {
        user: 'User',
        dateOfBirth: '5/4/2019',
        nhsNumber: '12346778',
      },
      appVersion: {
        webVersion: 'web',
      },
    };
  });
  const buildStoreGetters = ({
    notificationsEnabled = false,
    isProofLevel9 = true,
    biometricSupported = false,
    biometricType = 'fingerPrint',
  } = {}) => ({
    'serviceJourneyRules/notificationsEnabled': notificationsEnabled,
    'appVersion/isNativeVersionAfter': jest.fn().mockReturnValue(true),
    'session/isProofLevel9': isProofLevel9,
    'serviceJourneyRules/silverIntegrationEnabled': jest.fn().mockReturnValue(false),
    'loginSettings/biometricType': biometricType,
    'loginSettings/biometricRegistered': true,
    'loginSettings/biometricSupported': biometricSupported,
  });
  const createStyle = () => ({
    'list-menu': 'list-menu',
  });
  const mountPage = ({
    notificationsEnabled = false,
    isNativeApp = false,
    supportsLinkedProfiles = false,
    isProofLevel9 = true,
    biometricSupported = false,
    biometricType = 'fingerPrint',
  }) => {
    const $store = createStore({ state: $state });
    $store.getters = buildStoreGetters({
      notificationsEnabled,
      isProofLevel9,
      biometricSupported,
      biometricType,
    });
    $state.device.isNativeApp = isNativeApp;
    $state.serviceJourneyRules.rules.supportsLinkedProfiles = supportsLinkedProfiles;
    return mount(AccountAndSettingsPage, {
      $store,
      $state,
      $style: createStyle(),
      mountOpts: { i18n },
    });
  };
  function PageLinksAreDisplayed(isNative, biometricSupported, biometricType) {
    if (isNative) {
      if (biometricSupported === true && biometricType === 'fingerPrint') {
        it('will have a fingerprint link', () => {
          expect(wrapper.findAll('li').at(0).text()).toContain('Fingerprint');
        });
      }
      if (biometricSupported === true && biometricType === 'face') {
        it('will have a face link', () => {
          expect(wrapper.findAll('li').at(0).text()).toContain('Face');
        });
      }
      if (biometricSupported === false) {
        it('will have a login options link', () => {
          expect(wrapper.findAll('li').at(0).text()).toContain('Login options');
        });
      }
      it('will have a manage nhs account link', () => {
        expect(wrapper.findAll('li').at(1).text()).toContain('Manage NHS account');
      });
      it('will have a manage notifications link', () => {
        expect(wrapper.findAll('li').at(2).text()).toContain('Manage notifications');
      });
      it('will have a legal and cookies link', () => {
        expect(wrapper.findAll('li').at(3).text()).toContain('Legal and cookies');
      });
    } else {
      it('will have a manage nhs account link', () => {
        expect(wrapper.findAll('li').at(0).text()).toContain('Manage NHS account');
      });
      it('will have a legal and cookies link', () => {
        expect(wrapper.findAll('li').at(1).text()).toContain('Legal and cookies');
      });
    }
  }
  function ExecuteTest(isNative, biometricSupported, biometricType) {
    let backLink;
    beforeEach(() => {
      wrapper = mountPage({
        notificationsEnabled: true,
        isNativeApp: isNative,
        supportsLinkedProfiles: true,
        isProofLevel9: true,
        biometricSupported,
        biometricType,
      });
      backLink = wrapper.find('[data-purpose=back-link]');
    });
    PageLinksAreDisplayed(isNative, biometricSupported, biometricType);

    if (isNative) {
      it('will not have a back link', () => {
        expect(backLink.exists()).toBe(false);
      });
    } else {
      it('will have a back link', () => {
        expect(backLink.exists()).toBe(true);
      });
    }
  }
  describe('on a native app', () => {
    describe('with a fingerprint scanner', () => {
      ExecuteTest(true, true, 'fingerPrint');
    });
    describe('with a face scanner', () => {
      ExecuteTest(true, true, 'face');
    });
    describe('with undetermined biometric', () => {
      ExecuteTest(true, false);
    });
  });
  describe('on desktop', () => {
    ExecuteTest(false);
  });
});
