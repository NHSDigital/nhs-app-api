import ConfirmPrescription from '@/pages/prescriptions/confirm-prescription-details';
import { $t, createStore, mount } from '../../helpers';

describe('no nominated pharmacy summary', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: undefined,
      },
    },
    repeatPrescriptionCourses: {
      specialRequest: '',
      specialRequestNecessity: 'NotAllowed',
    },
  }) => state;

  const mountPage = () => mount(ConfirmPrescription, { $store, $t });

  describe('nominated pharmacy summary', () => {
    let pharmacyBlock;
    let pharmacySummary;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['repeatPrescriptionCourses/selectedPrescriptions'] = [{ courseId: 1 }];
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      pharmacyBlock = wrapper.find('#my-nominated-pharmacy');
      pharmacySummary = wrapper.find('#pharmacy-summary');
    });

    it('will not exist', () => {
      expect(pharmacyBlock.exists()).toBe(false);
    });

    it('will not have pharmacy summary', () => {
      expect(pharmacySummary.exists()).toBe(false);
    });
  });
});

describe('nominated pharmacy summary present', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'boots',
      },
    },
    repeatPrescriptionCourses: {
      specialRequest: '',
      specialRequestNecessity: 'NotAllowed',
    },
  }) => state;

  const mountPage = () => mount(ConfirmPrescription, { $store, $t });

  describe('nominated pharmacy summary', () => {
    let pharmacyBlock;
    let pharmacySummary;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters['repeatPrescriptionCourses/selectedPrescriptions'] = [{ courseId: 1 }];
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      wrapper = mountPage();
      pharmacyBlock = wrapper.find('#my-nominated-pharmacy');
      pharmacySummary = wrapper.find('#pharmacy-summary');
    });

    it('will exist', () => {
      expect(pharmacyBlock.exists()).toBe(true);
    });

    it('will have pharmacy summary', () => {
      expect(pharmacySummary.exists()).toBe(true);
    });
  });
});
