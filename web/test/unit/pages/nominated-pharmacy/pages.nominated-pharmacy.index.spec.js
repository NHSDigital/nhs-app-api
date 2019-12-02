import * as dependency from '@/lib/utils';
import { create$T, createStore, mount } from '../../helpers';
import { PRESCRIPTIONS } from '../../../../src/lib/routes';
import NominatedPharmacyIndex from '@/pages/nominated-pharmacy/index';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';

const $t = create$T();

describe('nominated pharmacy not found', () => {
  let $store;
  let $style;
  let $router;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {},
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyIndex, { $store, $style, $t, $router });

  describe('nominated pharmacy not found', () => {
    let backButton;
    let noNominatedPharmacyWarning;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $style = {
        button: 'button',
        grey: 'grey',
      };
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = true;
      wrapper = mountPage();
      backButton = wrapper.find('#back-button').find('a');
      noNominatedPharmacyWarning = wrapper.find(NoNominatedPharmacyWarning);
    });

    describe('nominated pharmacy warning', () => {
      it('will exist', () => {
        expect(noNominatedPharmacyWarning.exists()).toBe(true);
      });
    });

    it('change nominated pharmacy link not visible', () => {
      const linkToChange = wrapper.find('#link-to-change-pharmacy');
      expect(linkToChange.exists()).toBe(false);
    });

    describe('back-to-prescriptions', () => {
      it('will exist', () => {
        expect(backButton.exists()).toBe(true);
      });

      it('will use "nominatedPharmacyNotFound.backButton" for text', () => {
        expect(backButton.text())
          .toEqual('translate_generic.backButton.text');
      });

      it('will navigate back to prescriptions page', async () => {
        dependency.redirectTo = jest.fn();
        await backButton.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS.path);
      });
    });
  });
});


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
      const linkToChange = wrapper.find('#link-to-change-pharmacy');
      expect(linkToChange.exists()).toBe(true);
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

