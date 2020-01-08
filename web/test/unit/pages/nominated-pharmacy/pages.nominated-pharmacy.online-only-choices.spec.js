import { create$T, createStore, mount } from '../../helpers';
import NominatedPharmacyOnlineOnlyChoices from '@/pages/nominated-pharmacy/online-only-choices';
import RadioGroup from '@/components/RadioGroup';
import { NOMINATED_PHARMACY_INTERRUPT, NOMINATED_PHARMACY_CHOOSE_TYPE } from '@/lib/routes';
import * as dependency from '@/lib/utils';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $router;
  let wrapper;
  let continueButton;
  let backLink;
  let errorMessage;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {},
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyOnlineOnlyChoices, { $store, $t, $router });

  describe('online only choices page', () => {
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()),
        state: createState() });
      wrapper = mountPage();
      dependency.redirectTo = jest.fn();
      continueButton = wrapper.find('#continue-button');
      backLink = wrapper.find('#back-link').find('a');
    });

    describe('dispatches the results when option is selected and continue is clicked', () => {
      it('dispatches results to the store', async () => {
        wrapper.vm.hasTriedToContinue = true;
        wrapper.vm.onlineOnlyChoice = true;
        const expectedChoice = true;
        continueButton.trigger('click');
        errorMessage = wrapper.find('#error-message');
        expect(errorMessage.exists()).toBe(false);
        expect($store.dispatch)
          .toHaveBeenCalledWith('nominatedPharmacy/setOnlineOnlyKnownOption', expectedChoice);
      });
    });

    describe('continue-button', () => {
      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      it('will use "nominated_pharmacy.interrupt.continueButton" for text', () => {
        expect(continueButton.text())
          .toEqual('translate_nominatedPharmacyOnlineOnlyChoices.continueButton');
      });
      // todo: update with path to correct page
      it('will use redirect to PAGE X when yes is selected and the continue button is clicked', () => {
        wrapper.vm.hasTriedToContinue = true;
        wrapper.vm.onlineOnlyChoice = true;
        errorMessage = wrapper.find('#error-message');
        continueButton.trigger('click');
        expect(errorMessage.exists()).toBe(false);
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT.path);
      });
      // todo: update with path to correct page
      it('will use redirect to PAGE X when no is selected and the continue button is clicked', () => {
        wrapper.vm.hasTriedToContinue = true;
        wrapper.vm.onlineOnlyChoice = false;
        continueButton.trigger('click');
        errorMessage = wrapper.find('#error-message');
        expect(errorMessage.exists()).toBe(false);
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT.path);
      });

      it('will not redirect to next page in flow when a radio button has not been selected', () => {
        wrapper.vm.onlineOnlyChoice = undefined;
        continueButton.trigger('click');
        errorMessage = wrapper.find('#error-message');
        expect(errorMessage.exists()).toBe(true);
        expect($store.dispatch).not.toHaveBeenCalled();
        expect(dependency.redirectTo).not.toHaveBeenCalled();
      });
    });

    describe('back-link', () => {
      it('will exist on web', () => {
        expect(backLink.exists()).toBe(true);
      });

      it('will use "nominated_pharmacy.interrupt.continueButton" for text', () => {
        expect(backLink.text())
          .toEqual('translate_generic.backButton.text');
      });

      it('it will go back to the previous page set in the store', () => {
        backLink.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_CHOOSE_TYPE.path);
      });
    });

    describe('radio group', () => {
      it('will have radio buttons', () => {
        expect(wrapper.find(RadioGroup).exists()).toBe(true);
      });
    });
  });

  describe('online only choices error handling', () => {
    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      continueButton = wrapper.find('#continue-button');
      wrapper.vm.onlineOnlyChoice = undefined;
    });

    describe('error-message will appear when the user tries to continue and has not decided', () => {
      it('will exist', () => {
        continueButton.trigger('click');
        errorMessage = wrapper.find('#error-message');
        expect(errorMessage.exists()).toBe(true);
      });
    });
  });
});
