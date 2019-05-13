import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import { initialState } from '@/store/modules/nominatedPharmacy/mutation-types';
import { createStore, mount } from '../../helpers';

describe('pharmacy summary', () => {
  let $store;
  let wrapper;
  let props;

  beforeEach(() => {
    $store = createStore({
      state: {
        nominatedPharmacy: initialState(),
      },
    });

    props = {
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
    };

    wrapper = mount(PharmacySummary, {
      $store,
      propsData: props,
    });
  });

  describe('show nominated pharmacy name as header', () => {
    it('will exist', () => {
      const pharmacyName = wrapper.find('#pharmacyName');
      const pharmacyNameText = pharmacyName.find('h2');
      expect(pharmacyNameText.exists()).toBe(true);
      expect(pharmacyNameText.text()).toEqual('My Pharmacy');
    });
  });

  describe('will show nominated pharmacy name when pharmacyNameAsHeader is false', () => {
    it('will exist', () => {
      props.pharmacyNameAsHeader = false;
      wrapper = mount(PharmacySummary, {
        $store,
        propsData: props,
      });
      const pharmacyName = wrapper.find('#pharmacyName');
      const pharmacyNameText = pharmacyName.find('p');
      expect(pharmacyNameText.exists()).toBe(true);
      expect(pharmacyNameText.text()).toEqual('My Pharmacy');
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
