import { createStore, mount } from '../../helpers';
import NominatedPharmacyChooseType from '@/pages/nominated-pharmacy/choose-type';
import RadioGroup from '@/components/RadioGroup';
import { HIGH_STREET_PHARMACY, ONLINE_PHARMACY } from '@/store/modules/nominatedPharmacy/mutation-types';
import { NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES, NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_INTERRUPT } from '@/lib/routes';
import * as dependency from '@/lib/utils';

describe('nominated pharmacy choose type page', () => {
  let $router;
  let $store;
  let state;
  let wrapper;
  let continueButton;
  let backLink;
  let errorComponent;

  const createState = () => ({
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      chosenType: undefined,
    },
  });

  beforeEach(() => {
    state = createState();
    $store = createStore({ state });
    wrapper = mount(NominatedPharmacyChooseType, { $router, $store });
    continueButton = wrapper.find('#continue-button');
    dependency.redirectTo = jest.fn();
  });

  it('will have radio buttons', () => {
    expect(wrapper.find(RadioGroup).exists()).toBe(true);
  });

  describe('continue-button', () => {
    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will use "nominated_pharmacy.chooseType.buttonText" for text', () => {
      expect(continueButton.text())
        .toEqual('translate_nominated_pharmacy.chooseType.buttonText');
    });

    it('will redirect to the nominated pharmacy search page', () => {
      wrapper.vm.selected(HIGH_STREET_PHARMACY);
      continueButton.trigger('click');
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(false);
      expect($store.dispatch)
        .toHaveBeenCalledWith('nominatedPharmacy/setChosenType', HIGH_STREET_PHARMACY);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH.path);
    });

    it('will redirect to the online only choices page', () => {
      wrapper.vm.selected(ONLINE_PHARMACY);
      continueButton.trigger('click');
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(false);
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setChosenType', ONLINE_PHARMACY);
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm,
        NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.path);
    });

    it('will not redirect to next page in flow when no radio button is selected', () => {
      wrapper.vm.selected(undefined);
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
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT.path);
    });
  });
});
