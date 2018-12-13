import { mount, createLocalVue } from '@vue/test-utils';
import UpdatedTermsConditions from '@/components/UpdatedTermsConditions';
import Vuex from 'vuex';

const $t = key => `translate_${key}`;

const app = {
  $env: {
    TERMS_CONDITIONS_URL: 'http://example.com',
    PRIVACY_POLICY_URL: 'http://example.com',
    COOKIES_POLICY_URL: 'http://example.com',
  },
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
        hideDefaultCheckbox: false,
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

describe('UpdatedTermsConditions acceptance Process', () => {
  const $store = {
    state: {
      termsAndConditions: {
        areAccepted: false,
        acceptTerms: jest.fn(input => input),
      },
    },
    app,
  };

  const wrapper = createUpdatedTermsConditionsComponent($store);

  it('has T&Cs unchecked when loaded for the first time', () => {
    expect(wrapper.vm.areTermsAccepted).toBe(false);
  });

  it('registers terms as accepted', () => {
    wrapper.vm.check();
    expect(wrapper.vm.areTermsAccepted).toBe(true);
  });

  it('progresses when submit button clicked', () => {
    wrapper.vm.onConfirmButtonClicked().then(() => {
      expect($store.state.termsAndConditions.acceptTerms).toBeCalled();
    });
  });
});

describe('UpdatedTermsConditions error state', () => {
  const $store = {
    state: {
      termsAndConditions: {
        areAccepted: false,
      },
    },
    app,
  };

  const wrapper = createUpdatedTermsConditionsComponent($store);

  it('returns an error when terms are left unchecked', () => {
    wrapper.vm.onConfirmButtonClicked();
    expect(wrapper.vm.getErrorState()).toBe('mock validation border');
  });

  it('changes error state when terms checked', () => {
    wrapper.vm.check();
    wrapper.vm.onConfirmButtonClicked();
    expect(wrapper.vm.getErrorState()).toBeNull();
  });
});
