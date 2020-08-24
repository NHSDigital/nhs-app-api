import i18n from '@/plugins/i18n';
import PharmacyAddressComponent from '@/components/nominatedPharmacy/PharmacyAddressComponent';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import { createStore, mount } from '../../helpers';

const createPharmacyAddressComponent = ({ $store }) => mount(PharmacyAddressComponent, {
  propsData: {
    pharmacy: {
      pharmacyName: 'Best pharmacy',
      pharmacyType: 'P1',
      pharmacySubType: 'Community Pharmacy',
      telephoneNumber: '666668',
      addressLine1: 'address1',
      addressLine2: 'address2',
      addressLine3: 'address3',
      city: 'city',
      county: 'county',
      postcode: 'postcode',
      distance: '50',
    },
  },
  $store,
  mountOpts: {
    i18n,
  },
});


const createPharmacyAddressComponentForOnline = ({ $store }) => mount(PharmacyAddressComponent, {
  propsData: {
    pharmacy: {
      pharmacyName: 'Best pharmacy',
      pharmacyType: 'P1',
      pharmacySubType: 'Internet Pharmacy',
      telephoneNumber: '666668',
      addressLine1: '',
      addressLine2: '',
      addressLine3: '',
      city: '',
      county: '',
      postcode: '',
      url: 'http://www.testurl.com',
      distance: null,
    },
  },
  $store,
  mountOpts: {
    i18n,
  },
});

const createPharmacyAddressComponentForOnlineWithHttps =
  ({ $store }) => mount(PharmacyAddressComponent, {
    propsData: {
      pharmacy: {
        pharmacyName: 'Best pharmacy',
        pharmacyType: 'P1',
        pharmacySubType: 'Internet Pharmacy',
        telephoneNumber: '666668',
        addressLine1: '',
        addressLine2: '',
        addressLine3: '',
        city: '',
        county: '',
        postcode: '',
        url: 'https://www.testurl.com',
        distance: null,
      },
    },
    $store,
    mountOpts: {
      i18n,
    },
  });

describe('pharmacy address component', () => {
  let addressLine1;
  let addressLine2;
  let addressLine3;
  let city;
  let county;
  let postcode;
  let url;
  let telephoneNumber;
  let distanceAway;

  describe('pharmacy address component with a high street pharmacy', () => {
    let wrapper;
    let $store;

    beforeEach(() => {
      $store = createStore({
        state: {
          device: {
            source: 'android',
          },
          nominatedPharmacy: {
            chosenType: PharmacyTypeChoice.HIGH_STREET_PHARMACY,
          },
        },
      });
      wrapper = createPharmacyAddressComponent({ $store });
    });

    describe('main body content', () => {
      beforeEach(() => {
        addressLine1 = wrapper.find('#pharmacy-address-line-1');
        addressLine2 = wrapper.find('#pharmacy-address-line-2');
        addressLine3 = wrapper.find('#pharmacy-address-line-3');
        city = wrapper.find('#pharmacy-city');
        county = wrapper.find('#pharmacy-county');
        postcode = wrapper.find('#pharmacy-postcode');
        url = wrapper.find('#pharmacy-url');
        telephoneNumber = wrapper.find('#pharmacy-telephone-number');
        distanceAway = wrapper.find('#pharmacy-distance-away');
      });

      it('will exist', () => {
        expect(addressLine1.exists()).toEqual(true);
        expect(addressLine2.exists()).toEqual(true);
        expect(addressLine3.exists()).toEqual(true);
        expect(city.exists()).toEqual(true);
        expect(county.exists()).toEqual(true);
        expect(postcode.exists()).toEqual(true);
        expect(url.exists()).toEqual(false);
        expect(telephoneNumber.exists()).toEqual(true);
        expect(distanceAway.exists()).toEqual(true);
      });

      it('will have the correct values', () => {
        expect(addressLine1.text()).toEqual('address1');
        expect(addressLine2.text()).toEqual('address2');
        expect(addressLine3.text()).toEqual('address3');
        expect(city.text()).toEqual('city');
        expect(county.text()).toEqual('county');
        expect(postcode.text()).toEqual('postcode');
        expect(telephoneNumber.text()).toEqual('Telephone: 666668');
        expect(distanceAway.text()).toEqual('50 miles away');
      });
    });
  });

  describe('pharmacy address component with an online pharmacy', () => {
    let wrapper;
    let $store;

    beforeEach(() => {
      $store = createStore({
        state: {
          device: {
            source: 'android',
          },
          nominatedPharmacy: {
            chosenType: PharmacyTypeChoice.ONLINE_PHARMACY,
          },
        },
      });
      wrapper = createPharmacyAddressComponentForOnline({ $store });
    });

    describe('main body content', () => {
      beforeEach(() => {
        addressLine1 = wrapper.find('#pharmacy-address-line-1');
        addressLine2 = wrapper.find('#pharmacy-address-line-2');
        addressLine3 = wrapper.find('#pharmacy-address-line-3');
        city = wrapper.find('#pharmacy-city');
        county = wrapper.find('#pharmacy-county');
        postcode = wrapper.find('#pharmacy-postcode');
        url = wrapper.find('#pharmacy-url');
        telephoneNumber = wrapper.find('#pharmacy-telephone-number');
        distanceAway = wrapper.find('#pharmacy-distance-away');
      });

      it('will exist', () => {
        expect(addressLine1.exists()).toEqual(false);
        expect(addressLine2.exists()).toEqual(false);
        expect(addressLine3.exists()).toEqual(false);
        expect(city.exists()).toEqual(false);
        expect(county.exists()).toEqual(false);
        expect(postcode.exists()).toEqual(false);
        expect(url.exists()).toEqual(true);
        expect(telephoneNumber.exists()).toEqual(true);
        expect(distanceAway.exists()).toEqual(false);
      });

      it('will have the correct values', () => {
        expect(telephoneNumber.text()).toEqual('Telephone: 666668');
        expect(url.text()).toEqual('www.testurl.com');
      });
    });

    describe('url is handled correctly', () => {
      beforeEach(() => {
        url = wrapper.find('#pharmacy-url');
      });

      it('will remove the http:// when in the url data', () => {
        expect(url.text()).toEqual('www.testurl.com');
      });
    });
  });

  describe('pharmacy address component with an online pharmacy with https url', () => {
    let wrapper;
    let $store;

    beforeEach(() => {
      $store = createStore({
        state: {
          device: {
            source: 'android',
          },
          nominatedPharmacy: {
            chosenType: PharmacyTypeChoice.ONLINE_PHARMACY,
          },
        },
      });
      wrapper = createPharmacyAddressComponentForOnlineWithHttps({ $store });
    });

    describe('url will be handled correctly', () => {
      beforeEach(() => {
        url = wrapper.find('#pharmacy-url');
      });

      it('will remove the https in the url', () => {
        expect(url.text()).toEqual('www.testurl.com');
      });
    });
  });
});
