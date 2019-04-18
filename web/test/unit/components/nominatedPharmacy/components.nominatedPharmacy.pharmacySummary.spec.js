import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import { initialState } from '@/store/modules/nominatedPharmacy/mutation-types';
import { createStore, mount } from '../../helpers';

describe('pharmacy summary', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    $store = createStore({
      state: {
        nominatedPharmacy: initialState(),
      },
    });

    wrapper = mount(PharmacySummary, {
      $store,
      propsData: {
        pharmacy: {
          pharmacyName: 'My Pharmacy',
          addressLine1: '1 Stuart St',
          addressLine2: 'Brooklyn Avenue',
          addressLine3: 'Bangor',
          county: 'Greater London',
          city: 'London',
          postcode: 'SE254NQ',
          telephoneNumber: '01234567899',
        },
      },
    });
  });

  describe('show nominated pharmacy address', () => {
    let pharmacySummaryAddress;

    beforeEach(() => {
      pharmacySummaryAddress = wrapper.find('#address');
    });

    it('will exist', () => {
      expect(pharmacySummaryAddress.exists()).toBe(true);
    });

    it('will format the address', () => {
      expect(pharmacySummaryAddress.text()).toEqual('1 Stuart St, Brooklyn Avenue, ' +
        'Bangor, Greater London, London, SE254NQ');
    });
  });

  describe('show nominated pharmacy telephone number', () => {
    let pharmacySummaryPhoneNumber;

    beforeEach(() => {
      pharmacySummaryPhoneNumber = wrapper.find('#phoneNumber');
    });

    it('will exist', () => {
      expect(pharmacySummaryPhoneNumber.exists()).toBe(true);
    });

    it('will format the address', () => {
      expect(pharmacySummaryPhoneNumber.text()).toEqual('01234567899');
    });
  });
});
