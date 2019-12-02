import * as dependency from '@/lib/utils';
import { NOMINATED_PHARMACY, NOMINATED_PHARMACY_SEARCH_RESULTS } from '@/lib/routes';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import ConfirmNominatedPharmacy from '@/pages/nominated-pharmacy/confirm';
import { create$T, createStore, mount } from '../../helpers';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';

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

  const mountPage = () => mount(ConfirmNominatedPharmacy, { $store, $t });

  describe('nominated pharmacy details', () => {
    let pharmacyDetails;

    it('will exist', () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createState(),
      });
      wrapper = mountPage();
      pharmacyDetails = wrapper.find(PharmacyDetail);
      expect(pharmacyDetails.exists()).toBe(true);
    });

    it('will translate the line text for pharmacy', () => {
      const state = createState();
      state.nominatedPharmacy.selectedNominatedPharmacy.pharmacyType = PharmacyType.P1;
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state,
      });
      wrapper = mountPage();
      pharmacyDetails = wrapper.find(PharmacyDetail);
      expect($t).toHaveBeenCalledWith('nominated_pharmacy.confirm.line1');
    });

    it('will translate the line text for dispensing practice', () => {
      const state = createState();
      state.nominatedPharmacy.selectedNominatedPharmacy.pharmacyType = PharmacyType.P3;
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state,
      });
      wrapper = mountPage();
      pharmacyDetails = wrapper.find(PharmacyDetail);
      expect($t).toHaveBeenCalledWith('nominated_pharmacy.confirm.line1');
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
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(2, 'flashMessage/addSuccess', 'translate_nominated_pharmacy.confirm.pharmacyChosen');
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(3, 'nominatedPharmacy/clearSelectedNominatedPharmacy');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY.path);
    });

    it('will submit nominated pharmacy on click and call to redirect when changing an existing nominated pharmacy', async () => {
      dependency.redirectTo = jest.fn();
      $store.state.nominatedPharmacy.pharmacy.pharmacyName = 'Boots';
      $store.state.nominatedPharmacy.pharmacy.pharmacyType = 'P1';

      await confirmButton.trigger('click');
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(1, 'nominatedPharmacy/update', $store.state.nominatedPharmacy.selectedNominatedPharmacy.odsCode);
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(2, 'flashMessage/addSuccess', 'translate_nominated_pharmacy.confirm.pharmacyChanged');
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(3, 'nominatedPharmacy/clearSelectedNominatedPharmacy');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY.path);
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
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH_RESULTS.path);
    });
  });
});
