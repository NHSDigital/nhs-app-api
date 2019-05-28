import { create$T, createStore, mount } from '../../helpers';
import PrescriptionsPage from '@/pages/prescriptions/index';
import { PRESCRIPTION_REPEAT_COURSES, NOMINATED_PHARMACY_CHECK, NOMINATED_PHARMACY } from '../../../../src/lib/routes';

const $t = create$T();

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
  describe('nominated pharmacy will be shown', () => {
    let nominatedPharmacy;
    let nominatedPharmacyLink;
    let $store;
    let wrapper;

    beforeEach(() => {
      $store = createStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(true, true) },
      );

      const mountPage = () => mount(PrescriptionsPage, { $store, $t });
      wrapper = mountPage();
    });

    it('it will show pharmacy when it is a valid type', () => {
      nominatedPharmacy = wrapper.find('#nominated-pharmacy-section');
      expect(nominatedPharmacy.exists()).toBe(true);
    });

    it('it will redirect to nominated-pharmacy when clicked', () => {
      nominatedPharmacyLink = wrapper.find('#nominated-pharmacy');
      $store.app.$analytics = {
        trackButtonClick: jest.fn(),
      };

      wrapper.vm.onNominatedPharmacyDetailClicked();

      expect(nominatedPharmacyLink.exists()).toEqual(true);
      expect($store.app.$analytics.trackButtonClick)
        .toHaveBeenCalledWith(NOMINATED_PHARMACY.path, true);
    });
  });

  describe('nominated pharmacy will not be displayed', () => {
    let nominatedPharmacy;
    let $store;

    it('it will not show pharmacy when it is not a valid type', () => {
      $store = createStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(false, true) },
      );

      const mountPage = () => mount(PrescriptionsPage, { $store, $t });
      const wrapper = mountPage();
      nominatedPharmacy = wrapper.find('#nominated-pharmacy-section');
      expect(nominatedPharmacy.exists()).toBe(false);
    });
  });

  describe('continue button', () => {
    let $store;

    it('it will navigate to the courses page when it shouldnt show the nominated pharmacy', () => {
      $store = createStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(false, true) },
      );

      const mountPage = () => mount(PrescriptionsPage, { $store, $t });
      const wrapper = mountPage();
      const path = wrapper.vm.getContinueButtonPath();
      expect(path).toBe(PRESCRIPTION_REPEAT_COURSES.path);
    });

    it('it will have navigate to the nominated pharmacy check page when it should show the nominated pharmacy', () => {
      $store = createStore(
        { dispatch: jest.fn(() => Promise.resolve()), state: createState(true, true) },
      );

      const mountPage = () => mount(PrescriptionsPage, { $store, $t });
      const wrapper = mountPage();
      const path = wrapper.vm.getContinueButtonPath();
      expect(path).toBe(NOMINATED_PHARMACY_CHECK.path);
    });
  });
});

