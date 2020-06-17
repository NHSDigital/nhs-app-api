import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import { createStore, mount } from '../../helpers';

const createPharmacyDetailComponentForP1CommunityPharmacy = ({ $store }) => mount(PharmacyDetail, {
  propsData: {
    previousPath: '/myPreviousPath',
    isMyNominatedPharmacy: true,
    canChangePharmacy: true,
    showInstruction: true,
    displayChangeMyNominatedPharmacyButton: false,
    pharmacy: {
      pharmacyName: 'Best pharmacy',
      pharmacyType: 'P1',
      pharmacySubType: 'Community Pharmacy',
      telephoneNumber: '666668',
      addressLine1: 'address1',
      addressLine2: 'address2',
      addressLine3: 'address3',
      openingTimesFormatted: [{
        day: 'Sunday',
        times: [],
      }],
    },
  },
  $store,
});

const createPharmacyDetailComponentWhereDisplayChangeMyNominatedPharmacyButtonIsTrue =
  ({ $store }) => mount(PharmacyDetail, {
    propsData: {
      previousPath: '/myPreviousPath',
      isMyNominatedPharmacy: true,
      canChangePharmacy: true,
      showInstruction: true,
      displayChangeMyNominatedPharmacyButton: true,
      pharmacy: {
        pharmacyName: 'Best pharmacy',
        pharmacyType: 'P1',
        pharmacySubType: 'Community Pharmacy',
        telephoneNumber: '666668',
        addressLine1: 'address1',
        addressLine2: 'address2',
        addressLine3: 'address3',
        openingTimesFormatted: [{
          day: 'Sunday',
          times: [],
        }],
      },
    },
    $store,
  });

const createPharmacyDetailComponentForP1InternetPharmacy = ({ $store }) => mount(PharmacyDetail, {
  propsData: {
    previousPath: '/myPreviousPath',
    isMyNominatedPharmacy: true,
    canChangePharmacy: true,
    showInstruction: false,
    displayChangeMyNominatedPharmacyButton: false,
    pharmacy: {
      pharmacyName: 'Best pharmacy',
      pharmacyType: 'P1',
      pharmacySubType: 'Internet Pharmacy',
      telephoneNumber: '666668',
      addressLine1: 'address1',
      addressLine2: 'address2',
      addressLine3: 'address3',
      openingTimesFormatted: [{
        day: 'Sunday',
        times: [],
      }],
    },
  },
  $store,
});

const createPharmacyDetailComponentForP3 = ({ $store }) => mount(PharmacyDetail, {
  propsData: {
    previousPath: '/myPreviousPath',
    showInstruction: true,
    isMyNominatedPharmacy: true,
    canChangePharmacy: false,
    displayChangeMyNominatedPharmacyButton: false,
    pharmacy: {
      pharmacyName: 'Best practice',
      pharmacyType: 'P3',
      openingTimesFormatted: [{
        day: 'Sunday',
        times: [],
      }],
    },
  },
  $store,
});

describe('pharmacy detail', () => {
  describe('pharmacy detail with Pharmacy type as P1 and Pharmacy Subtype as Community Pharmacy', () => {
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
      wrapper = createPharmacyDetailComponentForP1CommunityPharmacy({ $store });
    });

    describe('change nominated pharmacy link', () => {
      let link;

      beforeEach(() => {
        link = wrapper.find('#link-to-change-pharmacy');
      });

      it('will display the link when displayChangeMyNominatedPharmacyButton is false', () => {
        expect(link.exists()).toEqual(true);
      });
    });

    describe('online only pharmacy detail component', () => {
      let onlineOnlyPharmacyDetail;

      beforeEach(() => {
        onlineOnlyPharmacyDetail = wrapper.find('#online-pharmacy-summary');
      });

      it('will not be displayed for community pharmacies', () => {
        expect(onlineOnlyPharmacyDetail.exists()).toEqual(false);
      });
    });

    describe('instruction about pharmacy', () => {
      let instruction;

      beforeEach(() => {
        instruction = wrapper.find('#instruction');
      });

      it('will display nominated pharmacy instruction', () => {
        expect(instruction.exists()).toEqual(true);
        expect(instruction.text()).toEqual('translate_nominated_pharmacy.confirm.line1');
      });
    });

    describe('opening times of the pharmacy', () => {
      let openingTime;

      beforeEach(() => {
        openingTime = wrapper.find('#pharmacy-opening-times');
      });

      it('will display the opening times', () => {
        expect(openingTime.exists()).toEqual(true);
      });
    });
  });

  describe('pharmacy detail with Pharmacy type as P1 and Pharmacy Subtype as Internet Pharmacy', () => {
    let wrapper;
    let $store;

    beforeEach(() => {
      $store = createStore({
        state: {
          device: {
            source: 'android',
          },
        },

        nominatedPharmacy: {
          chosenType: PharmacyTypeChoice.ONLINE_PHARMACY,
        },
      });
      wrapper = createPharmacyDetailComponentForP1InternetPharmacy({ $store });
    });

    describe('change nominated pharmacy link', () => {
      let link;
      let div;

      beforeEach(() => {
        link = wrapper.find('#link-to-change-pharmacy');
        div = wrapper.find('#internet-pharmacy-div');
      });

      it('will have bottom padding on the div', () => {
        expect(div.classes()).toContain('nhsuk-u-padding-bottom-5');
      });

      it('will display the link', () => {
        expect(link.exists()).toEqual(true);
      });
    });

    describe('instruction about pharmacy', () => {
      let instruction;

      beforeEach(() => {
        instruction = wrapper.find('#instruction');
      });

      it('will not display the instruction', () => {
        expect(instruction.exists()).toEqual(false);
      });
    });

    describe('online only pharmacy detail component', () => {
      let onlineOnlyPharmacyDetail;

      beforeEach(() => {
        onlineOnlyPharmacyDetail = wrapper.find('#online-pharmacy-summary');
      });

      it('will be displayed for community pharmacies', () => {
        expect(onlineOnlyPharmacyDetail.exists()).toEqual(true);
      });
    });

    describe('opening times of the pharmacy', () => {
      let openingTime;

      beforeEach(() => {
        openingTime = wrapper.find('#pharmacy-opening-times');
      });

      it('will not display the opening times', () => {
        expect(openingTime.exists()).toEqual(false);
      });
    });
  });

  describe('pharmacy detail with Pharmacy type as P3', () => {
    let wrapper;
    let $store;

    beforeEach(() => {
      $store = createStore({
        state: {
          device: {
            source: 'android',
          },
        },
      });
      wrapper = createPharmacyDetailComponentForP3({ $store });
    });

    describe('dispensing practice instruction', () => {
      let instruction;

      beforeEach(() => {
        instruction = wrapper.find('#instruction');
      });

      it('will display dispensing practice instruction', () => {
        expect(instruction.exists()).toEqual(true);
        expect(instruction.text()).toEqual('translate_nominated_pharmacy.confirm.line1');
      });
    });

    describe('change nominated pharmacy link will not be there', () => {
      let link;

      beforeEach(() => {
        link = wrapper.find('#link-to-change-pharmacy');
      });

      it('will not display the link', () => {
        expect(link.exists()).toEqual(false);
      });
    });
  });

  describe('pharmacy detail with displayChangeMyNominatedPharmacyButton as true', () => {
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
      wrapper =
        createPharmacyDetailComponentWhereDisplayChangeMyNominatedPharmacyButtonIsTrue({ $store });
    });

    describe('change nominated pharmacy button', () => {
      let button;

      beforeEach(() => {
        button = wrapper.find('#button-to-change-pharmacy');
      });

      it('will display the button to change nominated pharmacy when displayChangeMyNominatedPharmacyButton is true', () => {
        expect(button.exists()).toEqual(true);
      });
    });
  });
});
