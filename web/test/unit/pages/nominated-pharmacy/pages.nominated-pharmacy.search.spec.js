import * as dependency from '@/lib/utils';
import i18n from '@/plugins/i18n';
import SearchPharmacies from '@/pages/nominated-pharmacy/search';
import { NOMINATED_PHARMACY_SEARCH_RESULTS_PATH, NOMINATED_PHARMACY_CHOOSE_TYPE_PATH } from '@/router/paths';
import { initialState } from '@/store/modules/nominatedPharmacy/mutation-types';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

const $style = {};

const createState = () => ({
  nominatedPharmacy: initialState(),
  device: {
    source: 'web',
  },
});

const createHttp = () => ({
  getV1PatientPharmacies: jest.fn(),
});

const mountPage = ({ $http, $state = createState(), $store }) =>
  mount(
    SearchPharmacies,
    {
      $http,
      $store: ($store || createStore({ $http, $state })),
      $style,
      mountOpts: {
        i18n,
      },
    },
  );

describe('search pharmacies', () => {
  let $store;
  let $http;
  let page;
  let searchPharmaciesPage;
  const state = createState({ nominatedPharmacy: {} });

  beforeEach(() => {
    EventBus.$emit.mockClear();
    $http = createHttp();
    $store = createStore({
      $http,
      state,
      getters: {
        'nominatedPharmacy/previousPage': '/nominated-pharmacy',
        'nominatedPharmacy/nominatedPharmacyEnabled': true,
      },
      dispatch: jest.fn(() => Promise.resolve()),
    });
    page = mountPage({ $store, $http });
    searchPharmaciesPage = page.find(SearchPharmacies);
  });

  it('will exist', () => {
    expect(searchPharmaciesPage.exists()).toBe(true);
  });

  describe('searchClicked', () => {
    it('displays an error when search query is empty', async () => {
      // arrange
      page.vm.searchQuery = '';

      // act
      await page.vm.searchClicked();
      await page.vm.$nextTick();

      // assert
      expect(page.vm.showInvalidSearchError).toBe(true);
      expect($http.getV1PatientPharmacies).not.toHaveBeenCalled();
      expect(page.find('#empty-search-error').exists()).toBe(true);
      expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
    });

    it('displays an error when search query is populated but an invalid postcode', async () => {
      // arrange
      page.vm.searchQuery = 'rg';

      // act
      await page.vm.searchClicked();
      await page.vm.$nextTick();

      // assert
      expect(page.vm.showInvalidSearchError).toBe(true);
      expect($http.getV1PatientPharmacies).not.toHaveBeenCalled();
      expect(page.find('#empty-search-error').exists()).toBe(true);
    });

    it('displays a not found message when no pharmacies are returned', async () => {
      // arrange
      page.vm.searchQuery = 'rg1';
      $http.getV1PatientPharmacies.mockResolvedValue({
        pharmacies: [],
        pharmacyCount: null,
      });

      const expectedResult = {
        noResultsFound: true,
        pharmacies: [],
        technicalError: false,
      };

      // act
      await page.vm.searchClicked();

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchQuery', page.vm.searchQuery);
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchResults', expectedResult);
      expect(page.vm.showInvalidSearchError).toBe(false);
      expect(page.find('#empty-search-error').exists()).toBe(false);
      expect($http.getV1PatientPharmacies).toHaveBeenCalled();
      expect(page.vm.foundNoResultsMessage).toBe(`We could not find any results for "${page.vm.searchQuery}". Make sure you enter a valid full postcode in England.`);
    });
  });

  describe('when pharmacies are found', () => {
    it('sets noResultsFound to false and handles response', async () => {
      // arrange
      const testPostcode = 'rg1';
      const testPharmacies = [{ pharmacyName: 'boots' }];

      $http.getV1PatientPharmacies.mockResolvedValue({
        pharmacies: testPharmacies,
        pharmacyCount: null,
      });

      const expectedRequest = { searchTerm: testPostcode };
      const expectedResult = {
        noResultsFound: false,
        pharmacies: testPharmacies,
        technicalError: false,
      };

      // act
      const result = await page.vm.searchForPharmacies(testPostcode);

      // assert
      expect(result).not.toBeNull();
      expect(result).toEqual(expectedResult);
      expect($http.getV1PatientPharmacies).toHaveBeenCalledWith(expectedRequest);
    });
  });

  describe('when pharmacies are not found', () => {
    it('sets noResultsFound to true and handles response', async () => {
      // arrange
      $http.getV1PatientPharmacies.mockResolvedValue({ pharmacies: [], pharacyCount: null });
      const testPostcode = 'rg1';
      const expectedRequest = { searchTerm: testPostcode };
      const expectedResult = { noResultsFound: true, pharmacies: [], technicalError: false };

      // act
      const result = await page.vm.searchForPharmacies(testPostcode);

      // assert
      expect(result).not.toBeNull();
      expect(result).toEqual(expectedResult);
      expect($http.getV1PatientPharmacies).toHaveBeenCalledWith(expectedRequest);
    });
  });


  describe('after the search is done', () => {
    let testPostcode;
    let testPharmacies;
    let expectedResult;

    let processQuery;
    let searchForPharmacies;
    let validateSearchQuery;

    beforeEach(() => {
      // arrange
      dependency.redirectTo = jest.fn();

      searchForPharmacies = jest.fn();
      processQuery = jest.fn();
      validateSearchQuery = jest.fn();

      testPostcode = 'rg1';
      testPharmacies = [{ pharmacyName: 'boots' }];

      expectedResult = {
        noResultsFound: false,
        pharmacies: testPharmacies,
        technicalError: false,
      };

      $http.getV1PatientPharmacies.mockResolvedValue({
        pharmacies: testPharmacies,
        pharmacyCount: null,
      });

      processQuery.mockReturnValue('123');
      validateSearchQuery.mockReturnValue(true);
      searchForPharmacies.mockReturnValue(expectedResult);

      page.vm.searchQuery = testPostcode;
    });

    it('dispatches results to the store', async () => {
      // act
      await page.vm.searchClicked();

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchQuery', testPostcode);
      expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/setSearchResults', expectedResult);
    });

    it('navigates to the results page', async () => {
      // act
      dependency.redirectTo = jest.fn();
      await page.vm.searchClicked();

      // assert
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(page.vm, NOMINATED_PHARMACY_SEARCH_RESULTS_PATH);
    });
  });

  describe('after processQuery is run', () => {
    it('will have trimmed trailing whitespace', () => {
      const searchQuery = '  abc  ';
      const processedQuery = page.vm.processQuery(searchQuery);

      expect(processedQuery).toEqual('abc');
    });
  });

  describe('back link for desktop', () => {
    let backLink;

    beforeEach(() => {
      backLink = page.find('#back-link').find('a');
    });

    it('will exist', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('it will go back to the previous page set in the store', () => {
      dependency.redirectTo = jest.fn();
      backLink.trigger('click');

      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(page.vm, NOMINATED_PHARMACY_CHOOSE_TYPE_PATH);
    });
  });
});
