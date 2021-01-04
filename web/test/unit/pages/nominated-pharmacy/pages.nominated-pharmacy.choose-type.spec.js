import i18n from '@/plugins/i18n';
import NominatedPharmacyChooseType from '@/pages/nominated-pharmacy/choose-type';
import RadioGroup from '@/components/RadioGroup';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import {
  NOMINATED_PHARMACY_DSP_INTERRUPT_PATH,
  NOMINATED_PHARMACY_SEARCH_PATH,
  NOMINATED_PHARMACY_INTERRUPT_PATH,
} from '@/router/paths';
import * as dependency from '@/lib/utils';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

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
    EventBus.$emit.mockClear();
    getters = { 'nominatedPharmacy/nominatedPharmacyEnabled': true };
    state = createState();
    $store = createStore({ state, getters });
    dependency.redirectTo = jest.fn();
    wrapper = mount(
      NominatedPharmacyChooseType,
      {
        $router,
        $store,
        mountOpts: {
          i18n,
        },
      },
    );
    continueButton = wrapper.find('#continue-button');
  });

  it('will have radio buttons', () => {
    expect(wrapper.find(RadioGroup).exists()).toBe(true);
  });

  describe('continue-button', () => {
    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will display continue text', () => {
      expect(continueButton.text()).toEqual('Continue');
    });

    it('will redirect to the nominated pharmacy search page', async () => {
      wrapper.vm.selected(PharmacyTypeChoice.HIGH_STREET_PHARMACY);
      continueButton.trigger('click');
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(false);
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setChosenType', PharmacyTypeChoice.HIGH_STREET_PHARMACY);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH_PATH);
    });

    it('will redirect to the dsp interrupt page', async () => {
      wrapper.vm.selected(PharmacyTypeChoice.ONLINE_PHARMACY);
      continueButton.trigger('click');
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(false);
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setChosenType', PharmacyTypeChoice.ONLINE_PHARMACY);
      expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm,
        NOMINATED_PHARMACY_DSP_INTERRUPT_PATH);
    });

    it('will not redirect to next page in flow when no radio button is selected', async () => {
      wrapper.vm.selected(null);
      continueButton.trigger('click');
      await wrapper.vm.$nextTick();
      errorComponent = wrapper.find('#errorHeading');

      expect(errorComponent.exists()).toBe(true);
      expect($store.dispatch).not.toHaveBeenCalled();
      expect(dependency.redirectTo).not.toHaveBeenCalled();
      expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
    });
  });

  describe('Back link', () => {
    beforeEach(() => {
      backLink = wrapper.find('#back-link').find('a');
    });

    it('will exist', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('will display back text', () => {
      expect(backLink.text()).toEqual('Back');
    });

    it('will go to the interrupt page when clicked', () => {
      backLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT_PATH);
    });
  });
});
