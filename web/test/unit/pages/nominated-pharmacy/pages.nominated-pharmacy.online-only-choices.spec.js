import { create$T, createStore, mount } from '../../helpers';
import NominatedPharmacyOnlineOnlyChoices from '@/pages/nominated-pharmacy/online-only-choices';
import RadioGroup from '@/components/RadioGroup';
import {
  NOMINATED_PHARMACY_CHOOSE_TYPE,
  NOMINATED_PHARMACY_SEARCH_RESULTS,
  NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH,
} from '@/lib/routes';
import * as dependency from '@/lib/utils';

const $t = create$T();

describe('nominated pharmacy online only choices page', () => {
  let $store;
  let $router;
  let $http;
  let wrapper;
  let continueButton;
  let backLink;
  let errorMessage;

  const createHttp = () => ({
    getV1PatientOnlinePharmacies: jest.fn(() => Promise.resolve([{ pharmacyName: 'ABC' }])),
  });

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {},
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyOnlineOnlyChoices, { $store, $http, $t, $router });

  describe('online only choices page', () => {
    beforeEach(() => {
      dependency.redirectTo = jest.fn();
      $http = createHttp();
      $store = createStore({
        $http,
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
        getters: {
          'nominatedPharmacy/getOnlineOnlyKnownOption': null,
          'nominatedPharmacy/nominatedPharmacyEnabled': true,
        },
      });
      wrapper = mountPage();
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

      it('will use redirect to online only search page when yes is selected and the continue button is clicked', () => {
        wrapper.vm.hasTriedToContinue = true;
        wrapper.vm.onlineOnlyChoice = true;
        errorMessage = wrapper.find('#error-message');
        continueButton.trigger('click');
        expect(errorMessage.exists()).toBe(false);
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_ONLINE_ONLY_SEARCH.path);
      });

      it('will use redirect to search results when no is selected and the continue button is clicked', async () => {
        wrapper.vm.hasTriedToContinue = true;
        wrapper.vm.onlineOnlyChoice = false;
        // act
        await wrapper.vm.continueClicked();

        // assert
        errorMessage = wrapper.find('#error-message');
        expect(errorMessage.exists()).toBe(false);
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH_RESULTS.path);
      });

      it('will not redirect to next page in flow when a radio button has not been selected', () => {
        wrapper.vm.onlineOnlyChoice = null;
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
      wrapper.vm.onlineOnlyChoice = null;
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
