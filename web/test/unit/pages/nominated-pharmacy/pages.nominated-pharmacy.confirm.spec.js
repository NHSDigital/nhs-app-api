import * as dependency from '@/lib/utils';
import { NOMINATED_PHARMACY } from '@/lib/routes';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import ConfirmNominatedPharmacy from '@/pages/nominated-pharmacy/confirm';
import { create$T, createStore, mount } from '../../helpers';

const $t = create$T();

describe('confirm nominated pharmacy', () => {
  let $store;
  let $style;
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
      },
    },
  }) => state;

  const mountPage = () => mount(ConfirmNominatedPharmacy, { $store, $style, $t });

  describe('nominated pharmacy details', () => {
    let pharmacyDetails;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      pharmacyDetails = wrapper.find(PharmacyDetail);
    });

    it('will exist', () => {
      expect(pharmacyDetails.exists()).toBe(true);
    });

    it('will translate the line 1 text', () => {
      expect($t).toHaveBeenCalledWith('nominated_pharmacy.confirm.line1');
      expect($t).toHaveBeenCalledWith('nominated_pharmacy.line1');
    });
  });

  describe('confirm button', () => {
    let confirmButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      confirmButton = wrapper.find('#confirm-button');
      $style = {
        button: 'button',
        green: 'green',
      };
    });

    it('will exist', () => {
      expect(confirmButton.exists()).toBe(true);
    });

    it('will be a green button', () => {
      expect(confirmButton.classes()).toContain($style.green);
      expect(confirmButton.classes()).toContain($style.button);
    });

    it('will use "nominated_pharmacy.confirm.confirmButton" for text', () => {
      expect(confirmButton.text())
        .toEqual('translate_nominated_pharmacy.confirm.confirmButton');
    });

    it('will submit nominated pharmacy on click and call to redirect', async () => {
      dependency.redirectTo = jest.fn();
      await confirmButton.trigger('click');
      expect($store.dispatch).toHaveBeenNthCalledWith(1, 'nominatedPharmacy/update', 'RR123');
      expect($store.dispatch).toHaveBeenNthCalledWith(2, 'flashMessage/addSuccess', 'translate_nominated_pharmacy.confirm.pharmacyChanged');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY.path, null);
    });
  });
});
