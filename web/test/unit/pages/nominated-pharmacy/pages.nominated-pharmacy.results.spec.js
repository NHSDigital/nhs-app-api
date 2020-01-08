/* eslint-disable import/no-extraneous-dependencies */
import NominatedPharmacySearchResults from '@/pages/nominated-pharmacy/results';
import * as dependency from '@/lib/utils';
import { createLocalVue } from '@vue/test-utils';
import { create$T, createStore, mount } from '../../helpers';
import { NOMINATED_PHARMACY_SEARCH, NOMINATED_PHARMACY_CONFIRM } from '@/lib/routes';

const $tMock = create$T();

describe('nominated pharmacy search results', () => {
  let $store;
  let $style;
  let wrapper;
  let localVue;
  let pharmacySearchResults;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      searchQuery: '',
      searchResults: {
        pharmacies: [],
        technicalError: false,
        noResultsFound: true,
      },
    },
  }) => state;

  const mountPage = () => mount(NominatedPharmacySearchResults, {
    localVue,
    $store,
    $style,
    $t: (key) => {
      if (key === 'nominatedPharmacySearchResults.distanceAway') {
        return '{distance} miles away';
      }
      return $tMock(key);
    },
  });

  beforeEach(() => {
    localVue = createLocalVue();
    dependency.redirectTo = jest.fn();
    $store = createStore({
      dispatch: jest.fn(() => Promise.resolve()),
      state: createState(),
    });
    $store.getters['nominatedPharmacy/nominatedPharmacyEnabled'] = true;
  });

  it('will exist', () => {
    wrapper = mountPage();
    pharmacySearchResults = wrapper.find(NominatedPharmacySearchResults);
    expect(pharmacySearchResults.exists()).toBe(true);
  });

  it('will redirect to search if no search query present', () => {
    $store.state.nominatedPharmacy = {
      searchResults: {
        pharmacies: [],
        technicalError: false,
        noResultsFound: true,
      },
      nominatedPharmacyEnabled: true,
    };
    wrapper = mountPage();
    pharmacySearchResults = wrapper.find(NominatedPharmacySearchResults);
    expect(dependency.redirectTo)
      .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH.path);
  });

  it('will not redirect to search if a search query is present', () => {
    $store.state.nominatedPharmacy = {
      searchQuery: 'rg1',
      searchResults: {
        pharmacies: [],
        technicalError: false,
        noResultsFound: true,
      },
    };
    wrapper = mountPage();
    pharmacySearchResults = wrapper.find(NominatedPharmacySearchResults);
    expect(dependency.redirectTo).not.toHaveBeenCalled();
  });

  it('will show pharmacies when there are results found', () => {
    $store.state.nominatedPharmacy = {
      searchQuery: 'rg1',
      searchResults: {
        pharmacies: [{ pharmacyName: 'boots' }],
        technicalError: false,
        noResultsFound: false,
      },
    };
    wrapper = mountPage();
    pharmacySearchResults = wrapper.find(NominatedPharmacySearchResults);
    expect(dependency.redirectTo).not.toHaveBeenCalled();
  });

  it('will go back to the search page when the back button is clicked', () => {
    $store.state.nominatedPharmacy = {
      searchQuery: 'rg1',
      searchResults: {
        pharmacies: [{ pharmacyName: 'boots' }],
        technicalError: false,
        noResultsFound: false,
      },
    };
    wrapper = mountPage();
    pharmacySearchResults = wrapper.find(NominatedPharmacySearchResults);
    expect(dependency.redirectTo).not.toHaveBeenCalled();

    // act
    wrapper.vm.backButtonClicked();

    // assert
    expect(dependency.redirectTo)
      .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH.path);
  });

  describe('pharmacyPracticeClicked', () => {
    it('will put the selected pharmacy in the store and navigate to the confirm page', async () => {
      const pharmacy = { pharmacyName: 'drug store' };

      $store.state.nominatedPharmacy = {
        searchQuery: 'rg1',
        searchResults: {
          pharmacies: [pharmacy],
          technicalError: false,
          noResultsFound: false,
        },
      };
      wrapper = mountPage();
      pharmacySearchResults = wrapper.find(NominatedPharmacySearchResults);

      // act
      await wrapper.vm.pharmacyPracticeClicked(pharmacy);

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/select', pharmacy);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_CONFIRM.path);
    });
  });

  it('will display pharmacy details from the store', () => {
    const pharmacyDetail = {
      pharmacyName: 'drug store',
      odsCode: 'ABC1',
      addressLine1: 'address 1',
      addressLine2: 'address 2',
      addressLine3: 'address 3',
      city: 'city name',
      county: 'county name',
      postcode: 'ab1 2cd',
      telephoneNumber: '0819283',
      distance: '50',
    };

    $store.state.nominatedPharmacy = {
      searchQuery: 'rg1',
      searchResults: {
        pharmacies: [pharmacyDetail],
        technicalError: false,
        noResultsFound: false,
      },
    };
    wrapper = mountPage();
    pharmacySearchResults = wrapper.find(NominatedPharmacySearchResults);

    const pharmacyMenuItem = wrapper.find('#pharmacy-menu-item-0');
    // assert
    expect(pharmacyMenuItem.exists()).toBe(true);
    expect(pharmacyMenuItem.text()).toContain('drug store');
    expect(wrapper.find('#pharmacy-address-line-1').text()).toEqual(pharmacyDetail.addressLine1);
    expect(wrapper.find('#pharmacy-address-line-2').text()).toEqual(pharmacyDetail.addressLine2);
    expect(wrapper.find('#pharmacy-address-line-3').text()).toEqual(pharmacyDetail.addressLine3);
    expect(wrapper.find('#pharmacy-city').text()).toEqual(pharmacyDetail.city);
    expect(wrapper.find('#pharmacy-county').text()).toEqual(pharmacyDetail.county);
    expect(wrapper.find('#pharmacy-telephone-number').text()).toEqual(`translate_nominatedPharmacySearchResults.telephoneLabel${pharmacyDetail.telephoneNumber}`);
    expect(wrapper.find('#pharmacy-distance-away').text()).toEqual(`${pharmacyDetail.distance} miles away`);
  });
});
