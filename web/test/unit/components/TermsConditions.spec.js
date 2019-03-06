import { mount, createLocalVue } from '@vue/test-utils';
import TermsConditions from '@/components/TermsConditions';
import Vuex from 'vuex';
import { mount as mountHelper } from '../helpers';

const $t = key => `translate_${key}`;

const app = {
  $env: {
    TERMS_CONDITIONS_URL: 'http://example.com',
    PRIVACY_POLICY_URL: 'http://example.com',
    COOKIES_POLICY_URL: 'http://example.com',
  },
};

const createTermsConditionsComponent = ($store) => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  return mount(TermsConditions, {
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

describe('TermsConditions checkbox rendering', () => {
  let wrapper;

  beforeEach(() => {
    const mountTermsConditions = ({ state } = {}) =>
      mountHelper(TermsConditions, {
        state,
      });

    wrapper = mountTermsConditions({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    });
  });

  it('will verify that the tc agreement question checkbox has an associated label', () => {
    expect(wrapper.find("input[type='checkbox'][id='termsAndConditions-agree_checkbox']")
      .exists()).toEqual(true);

    expect(wrapper.find("label[for='termsAndConditions-agree_checkbox']")
      .exists()).toEqual(true);
  });

  it('will verify that the cookie consent checkbox has an associated label', () => {
    expect(wrapper.find("input[type='checkbox'][id='analyticsCookie-agree_analyticsCookieCheckbox']")
      .exists()).toEqual(true);

    expect(wrapper.find("label[for='analyticsCookie-agree_analyticsCookieCheckbox']")
      .exists()).toEqual(true);
  });
});

describe('TermsConditions acceptance', () => {
  const $store = {
    state: {
      termsAndConditions: {
        areAccepted: false,
        acceptTerms: jest.fn(input => input),
      },
    },
    dispatch: jest.fn(),
    app,
  };

  const wrapper = createTermsConditionsComponent($store);

  it('has T&Cs unchecked when loaded for the first time', () => {
    expect(wrapper.vm.areTermsAccepted).toBe(false);
  });

  it('has analytics unchecked when loaded for the first time', () => {
    expect(wrapper.vm.isAnalyticsCookieAccepted).toBe(false);
  });

  it('registers terms as accepted', () => {
    wrapper.vm.checkTerms();
    expect(wrapper.vm.areTermsAccepted).toBe(true);
  });

  it('registers analytics as accepted', () => {
    wrapper.vm.checkAnalyticsCookieAccepted();
    expect(wrapper.vm.isAnalyticsCookieAccepted).toBe(true);
  });

  it('progresses when submit button clicked', async () => {
    await wrapper.vm.onConfirmButtonClicked();
    expect($store.dispatch).toBeCalledWith('termsAndConditions/acceptTerms', {
      consentRequest: {
        ConsentGiven: true,
        AnalyticsCookieAccepted: true,
      },
    });
  });
});

describe('TermsConditions error state', () => {
  const $store = {
    state: {
      termsAndConditions: {
        areAccepted: false,
      },
    },
    app,
  };

  const wrapper = createTermsConditionsComponent($store);

  it('returns an error when terms are left unchecked', () => {
    wrapper.vm.onConfirmButtonClicked();
    expect(wrapper.vm.getErrorState()).toBe('mock validation border');
  });

  it('changes error state when terms checked', () => {
    wrapper.vm.checkTerms();
    wrapper.vm.onConfirmButtonClicked();
    expect(wrapper.vm.getErrorState()).toBeNull();
  });
});
