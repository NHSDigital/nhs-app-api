/* eslint-disable import/no-extraneous-dependencies */
import * as dependency from '@/lib/utils';
import SearchPharmacies from '@/pages/nominated-pharmacy/search';
import { initialState } from '@/store/modules/nominatedPharmacy/mutation-types';
import { createStore, mount, create$T } from '../../helpers';
import { NOMINATED_PHARMACY, NOMINATED_PHARMACY_SEARCH_RESULTS } from '@/lib/routes';

const $t = create$T();
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
    { $http, $store: ($store || createStore({ $http, $state })), $style, $t },
  );

describe('search pharmacies', () => {
  let $store;
  let $http;
  let page;
  let searchPharmaciesPage;
  const state = createState({ nominatedPharmacy: { previousPageToSearch: 'ben' } });

  beforeEach(() => {
    $http = createHttp();
    $store = createStore({
      $http,
      state,
      getters: {
        'nominatedPharmacy/previousPage': '/nominated-pharmacy',
      },
      dispatch: jest.fn(() => Promise.resolve()),
    });
    page = mountPage({ $store, $http });
    searchPharmaciesPage = page.find(SearchPharmacies);
  });

  it('will exist', () => {
    expect(searchPharmaciesPage.exists()).toBe(true);
  });

  it('will translate the line text', () => {
    expect($t).toHaveBeenCalledWith('nominated_pharmacy.search.line1');
    expect($t).toHaveBeenCalledWith('nominated_pharmacy.search.line2');
  });

  describe('when pharmacies are found', () => {
    it('sets noResultsFound to false and handles response', async () => {
      // arrange
      const testPostcode = 'rg1';
      const testPharmacies = [{ pharmacyName: 'boots' }];

      $http.getV1PatientPharmacies.mockResolvedValue(testPharmacies);

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
      $http.getV1PatientPharmacies.mockResolvedValue([]);
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

      $http.getV1PatientPharmacies.mockResolvedValue(testPharmacies);

      processQuery.mockReturnValue('123');
      validateSearchQuery.mockReturnValue(true);
      searchForPharmacies.mockReturnValue(expectedResult);

      page.vm.searchQuery = testPostcode;
    });

    it('disables the button', async () => {
      // act
      await page.vm.searchClicked();

      // assert
      expect(page.vm.isButtonDisabled).toBe(false);
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
        .toHaveBeenCalledWith(page.vm, NOMINATED_PHARMACY_SEARCH_RESULTS.path, null);
    });
  });

  describe('after processQuery is run', () => {
    it('will have trimmed trailing whitespace', () => {
      const searchQuery = '  abc  ';
      const processedQuery = page.vm.processQuery(searchQuery);

      expect(processedQuery).toEqual('abc');
    });
  });

  describe('back button', () => {
    let backButton;

    beforeEach(() => {
      backButton = page.find('#back-button');
    });

    it('will exist', () => {
      expect(backButton.exists()).toBe(true);
    });

    it('it will go back to the previous page set in the store', () => {
      dependency.redirectTo = jest.fn();
      backButton.trigger('click');

      expect(dependency.redirectTo).toHaveBeenCalledWith(page.vm, NOMINATED_PHARMACY.path, null);
    });
  });
});
