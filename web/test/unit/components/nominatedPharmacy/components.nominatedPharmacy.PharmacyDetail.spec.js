import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';

import { createStore, mount } from '../../helpers';

const createPharmacyDetailComponentForP1 = ({ $store }) => mount(PharmacyDetail, {
  propsData: {
    previousPath: '/myPreviousPath',
    isMyNominatedPharmacy: true,
    canChangePharmacy: true,
    pharmacy: {
      pharmacyName: 'Best pharmacy',
      pharmacyType: 'P1',
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
    isMyNominatedPharmacy: true,
    canChangePharmacy: false,
    pharmacy: {
      pharmacyName: 'Best pharmacy',
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
  describe('pharmacy detail with Pharmacy type as P1', () => {
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
      wrapper = createPharmacyDetailComponentForP1({ $store });
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
});
