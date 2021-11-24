import Amend from '@/pages/organ-donation/amend';
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import i18n from '@/plugins/i18n';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { INDEX_PATH, ORGAN_DONATION_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

const createState = ({ isAmending = false, isNativeApp = false } = {}) => ({
  device: {
    isNativeApp,
  },
  organDonation: {
    ...initialState(),
    ...{
      isAmending,
    },
  },
});

describe('organ donation amend page', () => {
  let $router;
  let $store;
  let state;
  let wrapper;

  const mountWrapper = (options) => {
    $router = createRouter();
    state = createState(options);
    $store = createStore({ state });

    return mount(Amend, {
      $router,
      $store,
      mountOpts: {
        i18n,
      },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('not native', () => {
    beforeEach(() => {
      wrapper = mountWrapper({ isAmending: false, isNativeApp: false });
    });

    it('will redirect back to the home page', () => {
      expect(redirectTo).toBeCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('native', () => {
    describe('is not amending', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ isAmending: false, isNativeApp: true });
      });

      it('will redirect back to the organ donation page', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
      });
    });

    describe('check amending', () => {
      describe('is not amending', () => {
        beforeEach(() => {
          wrapper = mountWrapper({ isAmending: false, isNativeApp: true });
        });

        it('will redirect back to the organ donation page', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
        });
      });

      describe('is amending', () => {
        beforeEach(() => {
          mountWrapper({ isAmending: true, isNativeApp: true });
        });

        it('will not redirect', () => {
          expect(redirectTo).not.toHaveBeenCalled();
        });
      });
    });

    describe('is amending', () => {
      let backLink;
      beforeEach(() => {
        wrapper = mountWrapper({ isAmending: true });
        backLink = wrapper.find('#back-link').find('a');
      });

      it('will show the "MakeDecision" component', () => {
        expect(wrapper.find(MakeDecision).exists()).toEqual(true);
      });

      it('will show the find out more link', () => {
        expect(wrapper.find(FindOutMoreLink).exists()).toEqual(true);
      });

      describe('button', () => {
        it('will exist', () => {
          expect(backLink.exists()).toBe(true);
        });

        it('will exist', () => {
          expect(backLink.exists()).toBe(true);
        });

        it('will display the correct text', () => {
          expect(backLink.text()).toEqual('Back');
        });
      });

      describe('click', () => {
        beforeEach(() => {
          backLink.trigger('click');
        });
        it('will dispatch the "amendCancel" event', () => {
          expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendCancel');
        });

        it('will push the organ donation page on the router', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
        });
      });
    });
  });
});

