/* eslint-disable import/no-extraneous-dependencies */
import * as dependency from '@/lib/utils';
import SearchPharmacies from '@/pages/nominated-pharmacy/search';
import { initialState } from '@/store/modules/nominatedPharmacy/mutation-types';
import { createStore, mount, $t } from '../../helpers';

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
    });
    page = mountPage({ $store, $http });
    searchPharmaciesPage = page.find(SearchPharmacies);
  });

  it('will exist', () => {
    expect(searchPharmaciesPage.exists()).toBe(true);
  });

  it('will translate the line text', () => {
    expect($t).toHaveBeenCalledWith('searchNominatedPharmacy.line1');
    expect($t).toHaveBeenCalledWith('searchNominatedPharmacy.line2');
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

      expect(dependency.redirectTo).toHaveBeenCalledWith(page.vm, '/nominated-pharmacy', null);
    });
  });
});
