import UpdatedTermsConditions from '@/components/UpdatedTermsConditions';
import { APPOINTMENTS, INDEX, REDIRECT_PARAMETER, TERMSANDCONDITIONS } from '@/lib/routes';
import { createRouter, createStore, mount } from '../helpers';

let $router;
let wrapper;
let $store;

const createUpdatedTermsConditionsComponent = ({ state, route = TERMSANDCONDITIONS }) => {
  $router = createRouter();
  $store = createStore({ state });

  return mount(UpdatedTermsConditions, {
    $route: route,
    $router,
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
  });
};

describe('UpdatedTermsConditions checkbox rendering', () => {
  beforeEach(() => {
    wrapper = createUpdatedTermsConditionsComponent({
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
  beforeEach(() => {
    const state = {
      termsAndConditions: {
        areAccepted: false,
        acceptTerms: jest.fn(input => input),
      },
      device: {
        isNativeApp: false,
      },
    };
    wrapper = createUpdatedTermsConditionsComponent({ state });
  });

  it('has T&Cs unchecked when loaded for the first time', () => {
    expect(wrapper.vm.areTermsAccepted).toBe(false);
  });

  describe('when terms accepted', () => {
    beforeEach(() => {
      wrapper.vm.areTermsAccepted = true;
    });

    describe('when the accept button is clicked', () => {
      beforeEach(() => {
        wrapper.find('#btn_accept').trigger('click');
      });

      it('progresses when submit button clicked', () => {
        expect($store.dispatch).toBeCalledWith('termsAndConditions/acceptTerms', {
          consentRequest: {
            ConsentGiven: true,
            UpdatingConsent: true,
          },
        });
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
    wrapper = createUpdatedTermsConditionsComponent(
      { state, route: { ...TERMSANDCONDITIONS, query: {} } },
    );
  });

  describe('current route has no redirect query param', () => {
    beforeEach(() => {
      wrapper.vm.areTermsAccepted = true;
      wrapper.vm.isAnalyticsCookieAccepted = true;
    });

    describe('when the accept button is clicked', () => {
      beforeEach(() => {
        wrapper.find('#btn_accept').trigger('click');
      });

      it('will redirect to INDEX route when the button is pushed', async () => {
        expect($router.push).toHaveBeenCalledWith(INDEX.path);
      });
    });
  });

  describe('current route has a redirect query parameter', () => {
    beforeEach(() => {
      const redirectRoute = {
        ...TERMSANDCONDITIONS,
        query: { [REDIRECT_PARAMETER]: APPOINTMENTS.name },
      };

      wrapper = createUpdatedTermsConditionsComponent({ state, route: redirectRoute });
      wrapper.vm.areTermsAccepted = true;
      wrapper.vm.isAnalyticsCookieAccepted = true;
    });

    describe('when the accept button is clicked', () => {
      beforeEach(() => {
        wrapper.find('#btn_accept').trigger('click');
      });

      it('will redirect to redirect parameter route when the button is pushed', async () => {
        expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
      });
    });
  });
});

describe('UpdatedTermsConditions error state', () => {
  beforeEach(() => {
    const state = {
      termsAndConditions: {
        areAccepted: false,
      },
      device: {
        isNativeApp: false,
      },
    };

    wrapper = createUpdatedTermsConditionsComponent({ state, route: TERMSANDCONDITIONS });
  });

  describe('when the accept button is clicked', () => {
    beforeEach(() => {
      wrapper.find('#btn_accept').trigger('click');
    });

    it('returns an error when terms are left unchecked', () => {
      expect(wrapper.vm.getErrorState()).toBe('mock validation border');
    });
  });

  describe('terms are accepted', () => {
    beforeEach(() => {
      wrapper.vm.areTermsAccepted = true;
      wrapper.find('#btn_accept').trigger('click');
    });

    it('changes error state when terms checked', () => {
      expect(wrapper.vm.getErrorState()).toBeNull();
    });
  });
});
