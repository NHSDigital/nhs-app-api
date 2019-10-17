import TermsAndConditions from '@/pages/terms-and-conditions';
import { create$T, createStore, initFilters, mount } from '../../helpers';

describe('Terms and Conditions', () => {
  describe('Initial Page', () => {
    initFilters();

    const $env = {};

    const $state = {
      device: {
        isNativeApp: false,
      },
      termsAndConditions: {
        areAccepted: false,
        updatedConsentRequired: false,
      },
    };

    describe('page title', () => {
      let $store;
      let $t;

      beforeEach(() => {
        $t = create$T();

        $store = createStore({ state: $state });
      });

      it('will not use updated title', () => {
        $state.device.isNativeApp = true;
        mount(TermsAndConditions, {
          $env,
          $t,
          $store,
          $state,
          $style: {},
        });

        expect($store.dispatch)
          .not.toBeCalledWith('header/updateHeaderText', 'translate_termsAndConditions.title');
      });
    });
  });

  describe('Updated Page', () => {
    initFilters();

    const $env = {};

    const $state = {
      device: {
        isNativeApp: false,
      },
      appVersion: {
        webVersion: 'web',
      },
      termsAndConditions: {
        areAccepted: true,
        updatedConsentRequired: true,
      },
    };

    describe('page title', () => {
      let $store;
      let $t;

      beforeEach(() => {
        $t = create$T();

        $store = createStore({ state: $state });
      });

      it('will use the updated title', () => {
        $state.device.isNativeApp = true;
        mount(TermsAndConditions, {
          $env,
          $t,
          $store,
          $state,
          $style: {},
        });

        expect($store.dispatch)
          .toBeCalledWith('header/updateHeaderText', 'translate_updatedTermsAndConditions.title');
      });
    });
  });
});
