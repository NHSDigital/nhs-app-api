import { create$T, createStore, mount } from '../../helpers';
import NominatedPharmacyIndex from '@/pages/nominated-pharmacy/index';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';

const $t = create$T();

describe('nominated pharmacy found', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = (type = PharmacyType.P1, state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'Boots',
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
        pharmacyType: type,
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyIndex, { $store, $style, $t });

  describe('show P1 pharmacy details', () => {
    let pharmacyDetails;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacyDetails = wrapper.find(PharmacyDetail);
    });

    it('will exist', () => {
      expect(pharmacyDetails.exists()).toBe(true);
      const buttonToChange = wrapper.find('#button-to-change-pharmacy');
      expect(buttonToChange.exists()).toBe(true);
    });
  });

  describe('show P3 pharmacy details', () => {
    let pharmacyDetails;

    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(PharmacyType.P3),
      });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacyDetails = wrapper.find(PharmacyDetail);
    });

    it('will exist, with no change pharmacy link', () => {
      expect(pharmacyDetails.exists()).toBe(true);
      const linkToChange = wrapper.find('#link-to-change-pharmacy');
      expect(linkToChange.exists()).toBe(false);
    });
  });
});

