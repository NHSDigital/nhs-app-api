import * as dependency from '@/lib/utils';
import { PRESCRIPTIONS } from '@/lib/routes';
import PharmacyChangeSuccessDetails from '@/components/nominatedPharmacy/PharmacyChangeSuccessDetails';
import NominatedPharmacyChangeSuccess from '@/pages/nominated-pharmacy/change-success';
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
        pharmacyType: PharmacyType.P1,
      },
      pharmacy: {
        pharmacyName: 'Boots',
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyChangeSuccess, { $store, $t });

  describe('nominated pharmacy change success details', () => {
    let pharmacyChangeSuccessDetails;

    it('will exist', async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createState(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
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
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS.path);
    });
  });
});
