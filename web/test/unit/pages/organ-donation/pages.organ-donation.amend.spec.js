import Amend from '@/pages/organ-donation/amend';
import MakeDecision from '@/components/organ-donation/MakeDecision';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION } from '@/lib/routes';
import { $t, createRouter, createStore, mount } from '../../helpers';

const createState = () => ({ organDonation: initialState() });

const createStyle = () => ({
  button: 'button',
  grey: 'grey',
});

describe('organ donation amend page', () => {
  let $router;
  let state;
  let $store;
  let $style;
  let wrapper;

  beforeEach(() => {
    $router = createRouter();
    state = createState();
    $store = createStore({ state });
    $style = createStyle();
    wrapper = mount(Amend, {
      $router,
      $store,
      $style,
      $t,
    });
  });

  describe('is not amending', () => {
    beforeEach(() => {
      state.organDonation.isAmending = false;
    });

    describe('fetch', () => {
      it('will redirect back to the organ donation page', () => {
        const redirect = jest.fn();
        wrapper.vm.$options.fetch({ redirect, store: $store });
        expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
      });
    });
  });

  describe('is amending', () => {
    beforeEach(() => {
      state.organDonation.isAmending = true;
    });

    it('will show the "MakeDecision" component', () => {
      expect(wrapper.find(MakeDecision).exists()).toEqual(true);
    });

    describe('back button', () => {
      let backButton;

      beforeEach(() => {
        backButton = wrapper.find('#back-button');
      });

      it('will exist', () => {
        expect(backButton.exists()).toEqual(true);
      });

      it('will be a grey button', () => {
        const classes = backButton.classes();
        expect(classes).toContain($style.grey);
        expect(classes).toContain($style.button);
      });

      it('will translate the generic back button text', () => {
        expect($t).toHaveBeenCalledWith('generic.backButton.text');
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

