import * as dependency from '@/lib/utils';
import { NOMINATED_PHARMACY_SEARCH_RESULTS_PATH, NOMINATED_PHARMACY_CHANGE_SUCCESS_PATH } from '@/router/paths';
import OnlineOnlyPharmacyDetail from '@/components/nominatedPharmacy/OnlineOnlyPharmacyDetail';
import ConfirmNominatedPharmacy from '@/pages/nominated-pharmacy/confirm';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import { create$T, createStore, mount } from '../../helpers';

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
        odsCode: 'RR123',
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
        pharmacyType: PharmacyType.P1,
      },
      pharmacy: {
      },
    },
  }) => state;

  const createStateWithP1Pharmacy = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      selectedNominatedPharmacy: {
        odsCode: 'RR123',
        pharmacyType: PharmacyType.P1,
        url: 'www.testurl.com',
        telephoneNumber: 91000000,
      },
      pharmacy: {
      },
    },
  }) => state;

  const mountPage = () => mount(ConfirmNominatedPharmacy, { $store, $t });

  describe('nominated pharmacy details when isHighStreetSelected is true', () => {
    let pharmacyOpeningTimes;
    let pharmacySummary;
    let pharmacyWarningMessage;

    it('will exist', () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createState(),
      });
      wrapper = mountPage();
      wrapper.vm.isHighStreetSelected = true;
      pharmacySummary = wrapper.find('#pharmacy-summary');
      pharmacyWarningMessage = wrapper.find('#confirm-help-text');
      pharmacyOpeningTimes = wrapper.find('#pharmacy-opening-times');
      expect(pharmacySummary.exists()).toBe(true);
      expect(pharmacyOpeningTimes.exists()).toBe(true);
      expect(pharmacyWarningMessage.exists()).toBe(true);
    });
  });

  describe('nominated pharmacy details when isOnlineOnlySelected is true', () => {
    let pharmacySummary;
    let onlinePharmacyDetails;
    let pharmacyOpeningTimes;
    let pharmacyWarningMessage;

    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createStateWithP1Pharmacy(),
      });
      wrapper = mountPage();
      wrapper.vm.isOnlineOnlySelected = true;
    });

    it('will not have the high street pharmacy details', () => {
      pharmacySummary = wrapper.find('#pharmacy-summary');
      pharmacyOpeningTimes = wrapper.find('#pharmacy-opening-times');
      expect(pharmacySummary.exists()).toBe(false);
      expect(pharmacyOpeningTimes.exists()).toBe(false);
    });

    it('will have the content for online only nominated pharmacy', () => {
      onlinePharmacyDetails = wrapper.find(OnlineOnlyPharmacyDetail);
      pharmacyWarningMessage = wrapper.find('#confirm-help-text');
      expect(onlinePharmacyDetails.exists()).toBe(true);
      expect(pharmacyWarningMessage.exists()).toBe(true);
    });
  });

  describe('nominated pharmacy details when isOnlineOnlySelected is true', () => {
    let onlinePharmacyDetails;
    let pharmacyOpeningTimes;
    let pharmacySummary;

    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createStateWithP1Pharmacy(),
      });
      wrapper = mountPage();
      wrapper.vm.isOnlineOnlySelected = true;
    });

    it('will not have the high street pharmacy details ', () => {
      pharmacySummary = wrapper.find('#pharmacy-summary');
      pharmacyOpeningTimes = wrapper.find('#pharmacy-opening-times');
      expect(pharmacySummary.exists()).toBe(false);
      expect(pharmacyOpeningTimes.exists()).toBe(false);
    });

    it('will have the content for online only nominated pharmacy', () => {
      onlinePharmacyDetails = wrapper.find(OnlineOnlyPharmacyDetail);
      expect(onlinePharmacyDetails.exists()).toBe(true);
    });
  });

  describe('confirm button for desktop', () => {
    let confirmButton;

    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      wrapper = mountPage();
      confirmButton = wrapper.find('#confirm-button');
    });

    it('will exist', () => {
      expect(confirmButton.exists()).toBe(true);
    });

    it('will be a button with nhsuk-button style', () => {
      const classes = confirmButton.classes();
      expect(classes).toContain('nhsuk-button');
    });

    it('will use "nominated_pharmacy.confirm.confirmButton" for text', () => {
      expect(confirmButton.text())
        .toEqual('translate_nominated_pharmacy.confirm.confirmButton');
    });

    it('will submit nominated pharmacy on click and call to redirect when pharmacy is being nominated for the first time', async () => {
      dependency.redirectTo = jest.fn();
      await confirmButton.trigger('click');
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(1, 'nominatedPharmacy/update', $store.state.nominatedPharmacy.selectedNominatedPharmacy.odsCode);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_CHANGE_SUCCESS_PATH);
    });

    it('will submit nominated pharmacy on click and call to redirect when changing an existing nominated pharmacy', async () => {
      dependency.redirectTo = jest.fn();
      $store.state.nominatedPharmacy.pharmacy.pharmacyName = 'Boots';
      $store.state.nominatedPharmacy.pharmacy.pharmacyType = 'P1';

      await confirmButton.trigger('click');
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(1, 'nominatedPharmacy/update', $store.state.nominatedPharmacy.selectedNominatedPharmacy.odsCode);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_CHANGE_SUCCESS_PATH);
    });
  });

  describe('confirm button for app', () => {
    let confirmButton;

    const createStateForApp = (state = {
      device: {
        source: 'ios',
      },
      nominatedPharmacy: {
        selectedNominatedPharmacy: {
          odsCode: 'RR123',
          openingTimesFormatted: [{
            day: 'Sunday',
            times: [],
          }],
        },
      },
    }) => state;

    const mountPageForApp = () => mount(ConfirmNominatedPharmacy, { $store, $t });

    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createStateForApp(),
      });
      wrapper = mountPageForApp();
      confirmButton = wrapper.find('#confirm-button');
    });

    it('will exist', () => {
      expect(confirmButton.exists()).toBe(true);
    });

    it('will be a button with nhsuk-button style', () => {
      const classes = confirmButton.classes();
      expect(classes).toContain('nhsuk-button');
    });

    it('will use "nominated_pharmacy.confirm.confirmButton" for text', () => {
      expect(confirmButton.text())
        .toEqual('translate_nominated_pharmacy.confirm.confirmButton');
    });
  });

  describe('back link for desktop', () => {
    let backLink;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      backLink = wrapper.find('#back-link').find('a');
    });

    it('will exist', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('will use "generic.backButton.text" for text', () => {
      expect(backLink.text())
        .toEqual('translate_generic.backButton.text');
    });

    it('will redirect to the search results page on click', async () => {
      dependency.redirectTo = jest.fn();
      await backLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH_RESULTS_PATH);
    });
  });
});
