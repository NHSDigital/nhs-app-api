import TermsConditions from '@/components/TermsConditions';
import * as LibUtils from '@/lib/utils';
import { TERMSANDCONDITIONS_NAME } from '@/router/names';
import { TERMSANDCONDITIONS_PATH } from '@/router/paths';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { createRouter, createStore, mount } from '../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

LibUtils.redirectTo = jest.fn();
LibUtils.redirectByName = jest.fn();

let $router;
let wrapper;
let $store;

const createTermsConditionsComponent = ({ state, query }) => {
  $router = createRouter();
  $store = createStore({ state });
  return mount(TermsConditions, {
    $store,
    $style: {
      validationText: 'mock validation test',
      info: 'info',
      customErrorBox: false,
      customErrorText: 'mock custom error text',
      button: 'btn',
      green: '#00ff00',
      validationBorderLeft: 'mock validation border',
    },
    $route: {
      path: TERMSANDCONDITIONS_PATH,
      name: TERMSANDCONDITIONS_NAME,
      query,
    },
    $router,
  });
};

beforeEach(() => {
  LibUtils.redirectTo.mockClear();
  LibUtils.redirectByName.mockClear();
  EventBus.$emit.mockClear();
});

describe('TermsConditions checkbox rendering', () => {
  beforeEach(() => {
    wrapper = createTermsConditionsComponent({
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
  beforeEach(() => {
    const state = {
      termsAndConditions: {
        areAccepted: false,
        acceptTerms: jest.fn(input => input),
      },
      device: { isNativeApp: false },
    };

    wrapper = createTermsConditionsComponent({ state });
  });

  it('has T&Cs unchecked when loaded for the first time', () => {
    expect(wrapper.vm.areTermsAccepted).toBe(false);
  });

  it('has analytics unchecked when loaded for the first time', () => {
    expect(wrapper.vm.isAnalyticsCookieAccepted).toBe(false);
  });

  describe('when both accepted', () => {
    beforeEach(() => {
      wrapper.vm.areTermsAccepted = true;
      wrapper.vm.isAnalyticsCookieAccepted = true;
    });

    it('updates the terms acceptance when submit button is clicked', async () => {
      wrapper.find('#btn_accept').trigger('click');
      await wrapper.vm.$nextTick();
      expect($store.dispatch).toBeCalledWith('termsAndConditions/acceptTerms', {
        consentRequest: {
          ConsentGiven: true,
          AnalyticsCookieAccepted: true,
        },
      });
    });
  });
});

describe('terms and conditions are accepted', () => {
  let state;

  beforeEach(() => {
    state = {
      termsAndConditions: {
        areAccepted: true,
        acceptTerms: jest.fn(input => input),
      },
      device: { isNativeApp: false },
    };
  });

  describe('current route has no redirect query param', () => {
    beforeEach(() => {
      wrapper = createTermsConditionsComponent({ state });
      wrapper.vm.areTermsAccepted = true;
      wrapper.vm.isAnalyticsCookieAccepted = true;
    });

    describe('when the accept button is clicked', () => {
      beforeEach(() => {
        wrapper.find('#btn_accept').trigger('click');
      });

      it('will dispatch "termsAndConditions/acceptTerms"', () => {
        expect($store.dispatch).toBeCalledWith('termsAndConditions/acceptTerms', {
          consentRequest: {
            ConsentGiven: true,
            AnalyticsCookieAccepted: true,
          },
        });
      });
    });
  });
});

describe('TermsConditions error state', () => {
  beforeEach(() => {
    const state = {
      termsAndConditions: {
        areAccepted: false,
      },
      device: {
        isNativeApp: false,
      },
    };

    wrapper = createTermsConditionsComponent({ state });
    wrapper.find('#btn_accept').trigger('click');
  });

  it('returns an error when terms are left unchecked', () => {
    expect(wrapper.vm.getErrorState()).toBe('mock validation border');
  });

  it('changes error state when terms checked', () => {
    wrapper.vm.areTermsAccepted = true;
    wrapper.vm.isAnalyticsCookieAccepted = true;
    expect(wrapper.vm.getErrorState()).toBeNull();
  });

  it('will set focus on the error component', () => {
    expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
  });
});
