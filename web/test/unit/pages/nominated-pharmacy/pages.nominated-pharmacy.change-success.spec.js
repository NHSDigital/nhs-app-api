import * as dependency from '@/lib/utils';
import { PRESCRIPTIONS_VIEW_ORDERS_PATH } from '@/router/paths';
import PharmacyChangeSuccessDetails from '@/components/nominatedPharmacy/PharmacyChangeSuccessDetails';
import OnlineOnlyPharmacyDetail from '@/components/nominatedPharmacy/OnlineOnlyPharmacyDetail';
import NominatedPharmacyChangeSuccess from '@/pages/nominated-pharmacy/change-success';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { create$T, createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

const $t = create$T();

describe('confirm nominated pharmacy', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      selectedNominatedPharmacy: {
        pharmacyType: PharmacyType.P1,
      },
      pharmacy: {
        pharmacyName: 'Boots',
        url: 'www.testurl.com',
      },
    },
  }) => state;

  const createStateWithNoUrl = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      selectedNominatedPharmacy: {
        pharmacyType: PharmacyType.P2,
      },
      pharmacy: {
        pharmacyName: 'Boots',
      },
    },
  }) => state;


  const mountPage = () => mount(NominatedPharmacyChangeSuccess, { $store, $t });

  describe('nominated pharmacy change success details for high street pharmacy', () => {
    let pharmacyChangeSuccessDetails;

    it('will exist', async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createState(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      wrapper.vm.isHighStreetSelected = true;
      pharmacyChangeSuccessDetails = wrapper.find(PharmacyChangeSuccessDetails);
      expect(pharmacyChangeSuccessDetails.exists()).toBe(true);
    });

    it('will translate the header for pharmacy', async () => {
      const state = createState();
      state.nominatedPharmacy.selectedNominatedPharmacy.pharmacyType = PharmacyType.P1;
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state,
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      pharmacyChangeSuccessDetails = wrapper.find(PharmacyChangeSuccessDetails);
      expect($t).toHaveBeenCalledWith('nominated_pharmacy.changeSuccess.header');
    });
  });

  describe('nominated pharmacy change success details for online only pharmacy with no url', () => {
    let onlinePharmacyChangeSuccessDetails;
    let postalWarning;
    let whatHappensNextWarning;
    let registrationWarningWithUrl;
    let registrationWarningWithNoUrl;

    beforeEach(async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createState(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      wrapper.vm.isOnlineOnlySelected = true;
      postalWarning = wrapper.find('#postal-warning');
      whatHappensNextWarning = wrapper.find('#what-happens-next');
      registrationWarningWithUrl = wrapper.find('#registrationWarningWithUrl');
      registrationWarningWithNoUrl = wrapper.find('#registrationWarningWithNoUrl');
    });

    it('will exist', async () => {
      onlinePharmacyChangeSuccessDetails = wrapper.find(OnlineOnlyPharmacyDetail);
      expect(onlinePharmacyChangeSuccessDetails.exists()).toBe(true);
    });

    it('will have the correct content when the pharmacy has url', async () => {
      expect(whatHappensNextWarning.exists()).toBe(true);
      expect(registrationWarningWithUrl.exists()).toBe(true);
      expect(registrationWarningWithNoUrl.exists()).toBe(false);
      expect(postalWarning.exists()).toBe(true);
    });

    it('will translate the correct locale variables for text', async () => {
      const pharmacyUrl = 'www.testurl.com';
      $store.state.nominatedPharmacy.pharmacy.url = pharmacyUrl;
      expect(whatHappensNextWarning.text()).toBe('translate_nominated_pharmacy.changeSuccess.whatHappensNext');
      expect(wrapper.find('#pharmacy-url').text()).toEqual(pharmacyUrl);
      expect(registrationWarningWithUrl.text()).toContain('translate_nominated_pharmacy.changeSuccess.registrationWarning');
      expect(registrationWarningWithUrl.text()).toContain(pharmacyUrl);
      expect(postalWarning.text()).toBe('translate_nominated_pharmacy.changeSuccess.postalWarning');
    });
  });

  describe('nominated pharmacy change success details for online only pharmacy with url', () => {
    let postalWarning;
    let whatHappensNextWarning;
    let registrationWarningWithUrl;
    let registrationWarningWithNoUrl;

    beforeEach(async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createStateWithNoUrl(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      wrapper.vm.isOnlineOnlySelected = true;
      postalWarning = wrapper.find('#postal-warning');
      whatHappensNextWarning = wrapper.find('#what-happens-next');
      registrationWarningWithUrl = wrapper.find('#registrationWarningWithUrl');
      registrationWarningWithNoUrl = wrapper.find('#registrationWarningWithNoUrl');
    });

    it('will have the correct content when the pharmacy has url', async () => {
      expect(whatHappensNextWarning.exists()).toBe(true);
      expect(registrationWarningWithUrl.exists()).toBe(false);
      expect(registrationWarningWithNoUrl.exists()).toBe(true);
      expect(postalWarning.exists()).toBe(true);
    });

    it('will translate the correct locale variables for text', async () => {
      expect(whatHappensNextWarning.text()).toBe('translate_nominated_pharmacy.changeSuccess.whatHappensNext');
      expect(registrationWarningWithNoUrl.text()).toBe('translate_nominated_pharmacy.changeSuccess.registrationWarningWithNoUrl');
      expect(postalWarning.text()).toBe('translate_nominated_pharmacy.changeSuccess.postalWarning');
    });
  });

  describe('go to prescriptions link for desktop', () => {
    let goToPrescriptionsLink;

    beforeEach(async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      goToPrescriptionsLink = wrapper.find('#to-prescriptions-link').find('a');
    });

    it('will exist', () => {
      expect(goToPrescriptionsLink.exists()).toBe(true);
    });

    it('will use "nominated_pharmacy.changeSuccess.linkLabel" for text', () => {
      expect(goToPrescriptionsLink.text())
        .toEqual('translate_nominated_pharmacy.changeSuccess.linkLabel');
    });

    it('will redirect to prescriptions page when clicked', async () => {
      dependency.redirectTo = jest.fn();
      await goToPrescriptionsLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_VIEW_ORDERS_PATH);
    });
  });

  describe('mounted', () => {
    beforeEach(() => {
      EventBus.$emit.mockClear();
      $t.mockClear();
      $store = createStore({ state: createState() });
      mountPage();
    });

    it('will emit UPDATE_HEADER with localised change success header as event', () => {
      expect($t).toHaveBeenCalledWith('pageHeaders.nominatedPharmacyChangeSuccess', { name: 'Boots' });
      expect(EventBus.$emit)
        .toHaveBeenCalledWith(UPDATE_HEADER, 'translate_pageHeaders.nominatedPharmacyChangeSuccess', true);
    });

    it('will emit UPDATE_TITLE with localised change success title as event', () => {
      expect($t).toHaveBeenCalledWith('pageTitles.nominatedPharmacyChangeSuccess', { name: 'Boots' });
      expect(EventBus.$emit)
        .toHaveBeenCalledWith(UPDATE_TITLE, 'translate_pageTitles.nominatedPharmacyChangeSuccess', true);
    });
  });
});
