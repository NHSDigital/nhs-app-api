import NominatedPharmacyChooseType from '@/pages/nominated-pharmacy/choose-type';
import RadioGroup from '@/components/RadioGroup';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import {
  NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
  NOMINATED_PHARMACY_SEARCH_PATH,
  NOMINATED_PHARMACY_INTERRUPT_PATH,
} from '@/router/paths';
import * as dependency from '@/lib/utils';
import { createStore, mount } from '../../helpers';

describe('nominated pharmacy choose type page', () => {
  let $router;
  let $store;
  let state;
  let wrapper;
  let continueButton;
  let backLink;
  let errorComponent;
  let getters;

  const createState = () => ({
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      chosenType: null,
    },
  });

  beforeEach(() => {
    getters = { 'nominatedPharmacy/nominatedPharmacyEnabled': true };
    state = createState();
    $store = createStore({ state, getters });
    dependency.redirectTo = jest.fn();
    wrapper = mount(NominatedPharmacyChooseType, { $router, $store });
    continueButton = wrapper.find('#continue-button');
  });

  it('will have radio buttons', () => {
    expect(wrapper.find(RadioGroup).exists()).toBe(true);
  });

  describe('continue-button', () => {
    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will use "nominated_pharmacy.chooseType.buttonText" for text', () => {
      expect(continueButton.text()).toEqual('translate_nominated_pharmacy.chooseType.buttonText');
    });

    it('will redirect to the nominated pharmacy search page', () => {
      wrapper.vm.selected(PharmacyTypeChoice.HIGH_STREET_PHARMACY);
      continueButton.trigger('click');
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(false);
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setChosenType', PharmacyTypeChoice.HIGH_STREET_PHARMACY);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH_PATH);
    });

    it('will redirect to the dsp interrupt page', () => {
      wrapper.vm.selected(PharmacyTypeChoice.ONLINE_PHARMACY);
      continueButton.trigger('click');
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(false);
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setChosenType', PharmacyTypeChoice.ONLINE_PHARMACY);
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm,
        NOMINATED_PHARMACY_DSP_INTERRUPT_PATH);
    });

    it('will not redirect to next page in flow when no radio button is selected', () => {
      wrapper.vm.selected(null);
      continueButton.trigger('click');
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(true);
      expect($store.dispatch).not.toHaveBeenCalled();
      expect(dependency.redirectTo).not.toHaveBeenCalled();
    });
  });

  describe('Back link', () => {
    beforeEach(() => {
      backLink = wrapper.find('#back-link').find('a');
    });

    it('will exist', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('will use "nominated_pharmacy.chooseType.backLinkText" for text', () => {
      expect(backLink.text())
        .toEqual('translate_nominated_pharmacy.chooseType.backLinkText');
    });

    it('will go to the interrupt page when clicked', () => {
      backLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT_PATH);
    });
  });
});
