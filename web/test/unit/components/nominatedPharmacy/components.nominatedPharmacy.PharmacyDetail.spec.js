import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';

import { createStore, mount } from '../../helpers';

const createPharmacyDetailComponent = ({ $store }) => mount(PharmacyDetail, {
  propsData: {
    previousPath: '/myPreviousPath',
    isMyNominatedPharmacy: true,
    pharmacy: {
      pharmacyName: 'Best pharmacy',
      telephoneNumber: '666668',
      addressLine1: 'address1',
      addressLine2: 'address2',
      addressLine3: 'address3',
      openingTimesFormatted: {},
    },
  },
  $store,
});

describe('pharmacy detail', () => {
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
    wrapper = createPharmacyDetailComponent({ $store });
  });

  describe('change nominated pharmacy link', () => {
    let link;

    beforeEach(() => {
      link = wrapper.find('#link_changeNominatedPharmacy');
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
