import Amend from '@/pages/organ-donation/amend';
import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createRouter, createStore, mount } from '../../helpers';
import { INDEX, ORGAN_DONATION } from '@/lib/routes';

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

  const mountWrapper = () => mount(Amend, {
    $router,
    $store,
  });

  beforeEach(() => {
    $router = createRouter();
    state = createState({ isAmending: true });
    $store = createStore({ state });
    wrapper = mountWrapper();
  });

  describe('fetch', () => {
    let redirect;

    const fetch = ({ isAmending, isNativeApp } = {}) => {
      redirect = jest.fn();

      wrapper.vm.$options.fetch({
        redirect,
        store: createStore({
          state: createState({ isAmending, isNativeApp }),
        }),
      });
    };

    describe('not native', () => {
      beforeEach(() => {
        fetch({ isAmending: false, isNativeApp: false });
      });

      it('will redirect back to the home page', () => {
        expect(redirect).toBeCalledWith(INDEX.path);
      });
    });

    describe('native', () => {
      describe('is not amending', () => {
        beforeEach(() => {
          fetch({ isAmending: false, isNativeApp: true });
        });

        it('will redirect back to the organ donation page', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('is amending', () => {
        beforeEach(() => {
          fetch({ isAmending: true, isNativeApp: true });
        });

        it('will not redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });
    });
  });

  describe('is amending', () => {
    beforeEach(() => {
      state = createState({ isAmending: true });
    });

    it('will show the "MakeDecision" component', () => {
      expect(wrapper.find(MakeDecision).exists()).toEqual(true);
    });

    it('will show the find out more link', () => {
      expect(wrapper.find(FindOutMoreLink).exists()).toEqual(true);
    });

    describe('back button', () => {
      let backButton;

      beforeEach(() => {
        backButton = wrapper.find('#back-button');
      });

      it('will exist', () => {
        expect(backButton.exists()).toEqual(true);
      });

      it('will be a button with a grey nhsuk-button style', () => {
        const classes = backButton.classes();
        expect(classes).toContain('nhsuk-button');
        expect(classes).toContain('nhsuk-button--secondary');
      });

      it('will translate the generic back button text', () => {
        expect(backButton.text()).toEqual('translate_generic.backButton.text');
      });

      describe('click', () => {
        beforeEach(() => {
          backButton.trigger('click');
        });

        it('will dispatch the "amendCancel" event', () => {
          expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendCancel');
        });

        it('will push the organ donation page on the router', () => {
          expect($router.push).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });
    });
  });
});

