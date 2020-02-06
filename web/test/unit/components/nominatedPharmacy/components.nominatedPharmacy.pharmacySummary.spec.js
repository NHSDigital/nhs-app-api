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
        device: 'web',
        nominatedPharmacy: initialState(),
      },
    });

    props = {
      pharmacy: {
        pharmacyName: 'My Pharmacy',
        url: 'http://www.myBestPharmacy.net',
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

  describe('nominated pharmacy address for community pharmacy', () => {
    let pharmacyAddressComponent;

    beforeEach(() => {
      props.pharmacy.pharmacySubType = 'Community Pharmacy';
      wrapper = mount(PharmacySummary, {
        $store,
        propsData: props,
      });
      pharmacyAddressComponent = wrapper.find('#pharmacy-address-component');
    });

    it('will display the address', () => {
      expect(pharmacyAddressComponent.exists()).toBe(true);
    });
  });

  describe('nominated pharmacy address for internet pharmacy', () => {
    let pharmacyAddressComponent;

    beforeEach(() => {
      props.pharmacy.pharmacySubType = 'Internet Pharmacy';
      wrapper = mount(PharmacySummary, {
        $store,
        propsData: props,
      });
      pharmacyAddressComponent = wrapper.find('#pharmacy-address-component');
    });

    it('will not be displayed', () => {
      expect(pharmacyAddressComponent.exists()).toBe(false);
    });
  });

  describe('will show nominated pharmacy name', () => {
    it('will exist', () => {
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
});
