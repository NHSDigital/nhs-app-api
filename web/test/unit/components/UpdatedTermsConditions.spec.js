import UpdatedTermsConditions from '@/components/UpdatedTermsConditions';
import Vuex from 'vuex';
import { mount, createLocalVue } from '@vue/test-utils';
import { create$T, createStore, mount as mountHelper } from '../helpers';

const $t = create$T();
const $env = {
  TERMS_CONDITIONS_URL: 'http://example.com',
  PRIVACY_POLICY_URL: 'http://example.com',
  COOKIES_POLICY_URL: 'http://example.com',
};

const createUpdatedTermsConditionsComponent = ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  return mount(UpdatedTermsConditions, {
    localVue,
    mocks: {
      $http,
      $store,
      $t,
      $style: {
        validationText: 'mock validation test',
        info: 'info',
        customErrorBox: false,
        customErrorText: 'mock custom error text',
        button: 'btn',
        green: '#00ff00',
        validationBorderLeft: 'mock validation border',
      },
    },
    showTemplate: () => true,
  });
};

describe('UpdatedTermsConditions checkbox rendering', () => {
  let wrapper;

  beforeEach(() => {
    const mountUpdatedTermsConditions = ({ state } = {}) =>
      mountHelper(UpdatedTermsConditions, {
        state,
      });

    wrapper = mountUpdatedTermsConditions({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
  });

  it('will verify that the updated tc agreement question checkbox has an associated label', () => {
    expect(wrapper.find("input[type='checkbox'][id='termsAndConditions-agree_checkbox']")
      .exists()).toEqual(true);

    expect(wrapper.find("label[for='termsAndConditions-agree_checkbox']")
      .exists()).toEqual(true);
  });
});

describe('UpdatedTermsConditions acceptance Process', () => {
  let wrapper;
  const $store = createStore({
    state: {
      termsAndConditions: {
        areAccepted: false,
        acceptTerms: jest.fn(input => input),
      },
    },
    $env,
  });

  beforeEach(() => {
    wrapper = createUpdatedTermsConditionsComponent($store);
  });

  it('has T&Cs unchecked when loaded for the first time', () => {
    expect(wrapper.vm.areTermsAccepted).toBe(false);
  });

  describe('when terms accepted', () => {
    beforeEach(() => {
      wrapper.vm.areTermsAccepted = true;
    });

    it('progresses when submit button clicked', () => {
      wrapper.vm.onConfirmButtonClicked();
      expect($store.dispatch).toBeCalledWith('termsAndConditions/acceptTerms', {
        consentRequest: {
          ConsentGiven: true,
          UpdatingConsent: true,
        },
      });
    });
  });
});

describe('UpdatedTermsConditions error state', () => {
  const $store = createStore({
    state: {
      termsAndConditions: {
        areAccepted: false,
      },
    },
    $env,
  });

  const wrapper = createUpdatedTermsConditionsComponent($store);

  it('returns an error when terms are left unchecked', () => {
    wrapper.vm.onConfirmButtonClicked();
    expect(wrapper.vm.getErrorState()).toBe('mock validation border');
  });

  it('changes error state when terms checked', () => {
    wrapper.vm.areTermsAccepted = true;
    wrapper.vm.onConfirmButtonClicked();
    expect(wrapper.vm.getErrorState()).toBeNull();
  });
});
