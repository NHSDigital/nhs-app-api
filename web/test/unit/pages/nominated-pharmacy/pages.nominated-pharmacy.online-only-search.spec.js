import { create$T, createStore, mount } from '../../helpers';
import NominatedPharmacyOnlineOnlySearch from '@/pages/nominated-pharmacy/online-only-search';
import * as dependency from '@/lib/utils';
import { NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES, NOMINATED_PHARMACY_SEARCH_RESULTS } from '@/lib/routes';

const $t = create$T();

describe('nominated pharmacy online only search page', () => {
  let $store;
  let $router;
  let wrapper;
  let searchButton;
  let backLink;
  let errorMessage;
  let searchInput;
  let noResultsFoundHelpText;
  let noResultsFoundHeader;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      pharmacy: {},
    },
  }) => state;

  describe('online only search page with no results', () => {
    const createHttp = () => ({
      getV1PatientOnlinePharmacies: jest.fn(() => Promise.resolve({
        pharmacies: [],
        pharmacyCount: 0,
      })),
    });

    const $http = createHttp();

    const mountPage = () => mount(NominatedPharmacyOnlineOnlySearch,
      { $store, $t, $router, $http });

    beforeEach(() => {
      $store = createStore({
        $http,
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      wrapper = mountPage();
      dependency.redirectTo = jest.fn();
      searchButton = wrapper.find('#search-button');
      backLink = wrapper.find('#back-link')
        .find('a');
      searchInput = wrapper.find('#searchTextInput');
      errorMessage = wrapper.find('#error-message');
    });

    describe('correct content is displayed initially', () => {
      it('search button', async () => {
        expect(searchButton.exists()).toBe(true);
        expect(searchButton.text())
          .toEqual('translate_nominatedPharmacyOnlineOnlySearch.searchButton');
      });

      it('input field', async () => {
        expect(searchInput.exists()).toBe(true);
      });

      it('back link', async () => {
        expect(backLink.exists()).toBe(true);
        expect(backLink.text())
          .toEqual('translate_generic.backButton.text');
      });

      it('error message hidden', async () => {
        expect(errorMessage.exists()).toBe(false);
      });
    });

    describe('back link', () => {
      it('navigates as expected when clicked', async () => {
        backLink.trigger('click');
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_ONLINE_ONLY_CHOICES.path);
      });
    });

    describe('no results found content', () => {
      it('is displayed when showMainPageHeader is false', async () => {
        wrapper.vm.showMainPageHeader = false;
        wrapper.vm.foundNoResultsMessage = 'message';
        noResultsFoundHelpText = wrapper.find('#noResultsFoundText');
        noResultsFoundHeader = wrapper.find('#noResultsFoundHeader');
        expect(noResultsFoundHelpText.exists()).toBe(true);
        expect(noResultsFoundHeader.exists()).toBe(true);
      });
    });

    describe('error message content', () => {
      it('is displayed when showErrorMessage is true', async () => {
        wrapper.vm.showErrorMessage = true;
        errorMessage = wrapper.find('#error-message');
        expect(errorMessage.exists()).toBe(true);
      });
    });

    describe('processQuery being called', () => {
      it('will have trimmed trailing whitespace', () => {
        const searchQuery = '  abc  ';
        const processedQuery = wrapper.vm.processQuery(searchQuery);

        expect(processedQuery).toEqual('abc');
      });
    });

    describe('searchButtonClicked', () => {
      it('displays an error when search query is empty', async () => {
        wrapper.vm.searchQuery = ' ';
        await wrapper.vm.searchButtonClicked();

        // assert
        expect(wrapper.vm.showErrorMessage).toBe(true);
        expect($http.getV1PatientOnlinePharmacies).not.toHaveBeenCalled();
        expect(wrapper.find('#invalid-search-term-error').exists()).toBe(true);
      });

      it('displays a not found message when no pharmacies are returned', async () => {
        wrapper.vm.searchQuery = 'XXXXXXX';
        const expectedResult = {
          noResultsFound: true,
          pharmacies: [],
          pharmacyCount: 0,
          technicalError: false,
        };
        await wrapper.vm.searchButtonClicked();
        expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchQuery', wrapper.vm.searchQuery);
        expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchResults', expectedResult);
        expect(wrapper.vm.showErrorMessage).toBe(false);
        expect(wrapper.find('#invalid-search-term-error').exists()).toBe(false);
        expect($http.getV1PatientOnlinePharmacies).toHaveBeenCalled();
        expect(wrapper.vm.noResultsFoundMessage).toBe('translate_nominatedPharmacyOnlineOnlySearch.noResultsHelpText');
      });
    });
  });

  describe('online only search page with results', () => {
    const response = {
      pharmacies: [
        { pharmacyName: 'abc' },
      ],
    };
    const createHttpWithResults = () => ({
      getV1PatientOnlinePharmacies: jest.fn(() => Promise.resolve(response)),
    });
    const $http = createHttpWithResults();
    const mountPageWithResults = () => mount(NominatedPharmacyOnlineOnlySearch,
      { $store, $t, $router, $http });

    beforeEach(() => {
      $store = createStore({
        $http,
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      wrapper = mountPageWithResults();
      dependency.redirectTo = jest.fn();
    });

    describe('searchButtonClicked', () => {
      it('will set the results correctly and navigate to the results page', async () => {
        wrapper.vm.searchQuery = '       abc      ';
        const expectedSearchQueryAfterTrim = 'abc';
        const expectedResult = {
          noResultsFound: false,
          pharmacies: [{ pharmacyName: 'abc' }],
          technicalError: false,
        };
        await wrapper.vm.searchButtonClicked();
        expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchQuery', expectedSearchQueryAfterTrim);
        expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchResults', expectedResult);
        expect(wrapper.vm.showErrorMessage).toBe(false);
        expect($http.getV1PatientOnlinePharmacies).toHaveBeenCalled();
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_SEARCH_RESULTS.path);
      });
    });
  });
});
