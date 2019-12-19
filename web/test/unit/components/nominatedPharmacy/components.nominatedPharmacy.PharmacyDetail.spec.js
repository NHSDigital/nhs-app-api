import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';

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

      describe('click', () => {
        it('will dispatch "nominatedPharmacy/setPreviousPageToSearch"', () => {
          wrapper.vm.goToChangeNominatedPharmacySearch();
          expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setPreviousPageToSearch', '/myPreviousPath');
        });
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

    describe('statement about internet pharmacy', () => {
      let statement;

      beforeEach(() => {
        statement = wrapper.find('#statement');
      });

      it('will not display the statement', () => {
        expect(statement.exists()).toEqual(false);
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
      });
      wrapper = createPharmacyDetailComponentForP1InternetPharmacy({ $store });
    });

    describe('change nominated pharmacy link', () => {
      let link;

      beforeEach(() => {
        link = wrapper.find('#link-to-change-pharmacy');
      });

      it('will display the link', () => {
        expect(link.exists()).toEqual(true);
      });

      describe('click', () => {
        it('will dispatch "nominatedPharmacy/setPreviousPageToSearch"', () => {
          wrapper.vm.goToChangeNominatedPharmacySearch();
          expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setPreviousPageToSearch', '/myPreviousPath');
        });
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

    describe('statement about internet pharmacy', () => {
      let statement;

      beforeEach(() => {
        statement = wrapper.find('#statement');
      });

      it('will display the statement', () => {
        expect(statement.exists()).toEqual(true);
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
