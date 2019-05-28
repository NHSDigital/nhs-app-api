import NoNominatedPharmacyChange from '@/pages/nominated-pharmacy/cannot-change';
import { create$T, createStore, mount } from '../../helpers';

const $t = create$T();

describe('can not change nominated pharmacy', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'Boots',
        pharmacyType: 'P3',
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
      },
    },
  }) => state;

  const mountPage = () => mount(NoNominatedPharmacyChange, { $store, $style, $t });

  describe('show-pharmacy-summary', () => {
    let pharmacySummary;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacySummary = wrapper.find('#pharmacy-summary');
    });

    it('will exist', () => {
      expect(pharmacySummary.exists()).toBe(true);
    });
  });

  describe('show-pharmacy-opening-times', () => {
    let pharmacyOpeningTimes;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacyOpeningTimes = wrapper.find('#pharmacy-opening-times');
    });

    it('will exist', () => {
      expect(pharmacyOpeningTimes.exists()).toBe(true);
    });
  });
});
