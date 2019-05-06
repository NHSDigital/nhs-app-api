import { $t, createStore as newStore, mount as newMount } from '../../helpers';
import PrescriptionsPage from '@/pages/prescriptions/index';

const createState = (isValid, hasLoaded) => ({
  device: {
    source: 'web',
  },
  prescriptions: {
    hasLoaded,
    prescriptionCourses: {},
  },
  nominatedPharmacy: {
    pharmacy: {
      pharmacyName: undefined,
    },
    nominatedPharmacyEnabled: isValid,
  },
});

describe('prescriptions/index.vue -', () => {
  describe('nominated pharmacy', () => {
    let nominatedPharmacy;
    let $store;

    it('it will show pharmacy when it is a valid type', () => {
      $store = newStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(true, true) },
      );

      const mountPage = () => newMount(PrescriptionsPage, { $store, $t });
      const newWrapper = mountPage();
      nominatedPharmacy = newWrapper.find('#nominated-pharmacy-section');
      expect(nominatedPharmacy.exists()).toBe(true);
    });

    it('it will show pharmacy when it is not a valid type', () => {
      $store = newStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(false, true) },
      );

      const mountPage = () => newMount(PrescriptionsPage, { $store, $t });
      const newWrapper = mountPage();
      nominatedPharmacy = newWrapper.find('#nominated-pharmacy-section');
      expect(nominatedPharmacy.exists()).toBe(false);
    });
  });

  describe('continue button', () => {
    let $store;

    it('it will navigate to the courses page when it shouldnt show the nominated pharmacy', () => {
      $store = newStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(false, true) },
      );

      const mountPage = () => newMount(PrescriptionsPage, { $store, $t });
      const newWrapper = mountPage();
      const path = newWrapper.vm.getContinueButtonPath();
      expect(path).toBe('/prescriptions/repeat-courses');
    });

    it('it will have navigate to the nominated pharmacy check page when it should show the nominated pharmacy', () => {
      $store = newStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(true, true) },
      );

      const mountPage = () => newMount(PrescriptionsPage, { $store, $t });
      const newWrapper = mountPage();
      const path = newWrapper.vm.getContinueButtonPath();
      expect(path).toBe('/nominated-pharmacy/check');
    });
  });
});

