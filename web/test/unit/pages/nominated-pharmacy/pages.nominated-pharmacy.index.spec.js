import * as dependency from '@/lib/utils';
import NominatedPharmacyIndex from '@/pages/nominated-pharmacy/index';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import { $t, createStore, mount } from '../../helpers';
import { PRESCRIPTIONS } from '../../../../src/lib/routes';

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
      backButton = wrapper.find('#back-button');
      noNominatedPharmacyWarning = wrapper.find(NoNominatedPharmacyWarning);
    });

    describe('nominated pharmacy warning', () => {
      it('will exist', () => {
        expect(noNominatedPharmacyWarning.exists()).toBe(true);
      });
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
          .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS.path, null);
      });
    });
  });
});


describe('nominated pharmacy found', () => {
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
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacyIndex, { $store, $style, $t });

  describe('show pharmacy details', () => {
    let pharmacyDetails;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['nominatedPharmacy/hasNoNominatedPharmacy'] = false;
      pharmacyDetails = wrapper.find(PharmacyDetail);
    });

    it('will exist', () => {
      expect(pharmacyDetails.exists()).toBe(true);
    });
  });
});

