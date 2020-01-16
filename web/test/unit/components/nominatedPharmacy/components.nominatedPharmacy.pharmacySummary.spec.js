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
    let pharmacySummaryAddress;

    beforeEach(() => {
      props.pharmacy.pharmacySubType = 'Community Pharmacy';
      wrapper = mount(PharmacySummary, {
        $store,
        propsData: props,
      });
      pharmacySummaryAddress = wrapper.find('#pharmacyAddress');
    });

    it('will display the address', () => {
      expect(pharmacySummaryAddress.exists()).toBe(true);
    });

    it('will format the address', () => {
      expect(pharmacySummaryAddress.text()).toEqual('1 Stuart St, Brooklyn Avenue, ' +
        'Bangor, London, Greater London, SE254NQ');
    });
  });

  describe('nominated pharmacy website url for community pharmacy', () => {
    let pharmacyWebsiteUrl;

    beforeEach(() => {
      props.pharmacy.pharmacySubType = 'Community Pharmacy';
      wrapper = mount(PharmacySummary, {
        $store,
        propsData: props,
      });
      pharmacyWebsiteUrl = wrapper.find('#url');
    });

    it('will not be displayed', () => {
      expect(pharmacyWebsiteUrl.exists()).toBe(false);
    });
  });

  describe('nominated pharmacy address for internet pharmacy', () => {
    let pharmacySummaryAddress;

    beforeEach(() => {
      props.pharmacy.pharmacySubType = 'Internet Pharmacy';
      wrapper = mount(PharmacySummary, {
        $store,
        propsData: props,
      });
      pharmacySummaryAddress = wrapper.find('#address');
    });

    it('will not be displayed', () => {
      expect(pharmacySummaryAddress.exists()).toBe(false);
    });
  });

  describe('nominated pharmacy website url for internet pharmacy', () => {
    let pharmacyWebsiteUrl;

    beforeEach(() => {
      props.pharmacy.pharmacySubType = 'Internet Pharmacy';
      wrapper = mount(PharmacySummary, {
        $store,
        propsData: props,
      });
      pharmacyWebsiteUrl = wrapper.find('#url');
    });

    it('will be displayed', () => {
      expect(pharmacyWebsiteUrl.exists()).toBe(true);
    });

    it('will have target set to blank', () => {
      expect(pharmacyWebsiteUrl.attributes().target).toEqual('_blank');
    });

    it('will go to share decision external url', () => {
      expect(pharmacyWebsiteUrl.attributes().href).toEqual('//http://www.myBestPharmacy.net');
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
});
