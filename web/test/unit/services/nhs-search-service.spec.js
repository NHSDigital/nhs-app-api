/* eslint-disable global-require, no-console, no-unused-vars */
import NHSSearchService from '@/services/nhs-search-service';
import axiosMock from 'axios';

const GP_LOOKUP_API_KEY = 'nhs_search_service_api_key';
const GP_LOOKUP_API_RESULTS_LIMIT = 20;
const GP_LOOKUP_API_URL = 'gp_lookup_url';
const POSTCODE_LOOKUP_API_URL = 'postcode_lookup_url';
const POSTCODE_LOOKUP_SEARCH_RADIUS_KM = 10;

const ORGANISATIONS_SEARCH_FIELDS = 'OrganisationName,Address2,Address3,City';
const ORGANISATIONS_SELECT_FIELDS = 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode';
const ORGANISATIONS_TYPE_ID_FILTER = 'OrganisationTypeID eq \'GPB\'';
const ORGANISATIONS_QUERY_TYPE = 'simple';
const getGeoDistanceFilter = (lat, lon) => `OrganisationTypeID eq 'GPB' and geo.distance(Geocode, geography'POINT(${lon} ${lat})') le ${POSTCODE_LOOKUP_SEARCH_RADIUS_KM}`;
const getGeoDistanceOrderBy = (lat, lon) => `geo.distance(Geocode, geography'POINT(${lon} ${lat})')`;

const defaultNHSSearchServiceRequest = {
  url: undefined,
  headers: {
    'subscription-key': GP_LOOKUP_API_KEY,
    'Content-Type': 'application/json',
  },
  data: undefined,
  method: 'POST',
};

describe('NHS Search Service', () => {
  beforeAll(() => {
    process.env.GP_LOOKUP_API_RESULTS_LIMIT = GP_LOOKUP_API_RESULTS_LIMIT;
    process.env.GP_LOOKUP_API_KEY = GP_LOOKUP_API_KEY;
    process.env.GP_LOOKUP_API_URL = GP_LOOKUP_API_URL;
    process.env.POSTCODE_LOOKUP_API_URL = POSTCODE_LOOKUP_API_URL;
    process.env.POSTCODE_LOOKUP_SEARCH_RADIUS_KM = POSTCODE_LOOKUP_SEARCH_RADIUS_KM;
  });
  describe('searchGPPractices', () => {
    beforeEach(() => {
      jest.clearAllMocks();
    });
    it('will call NHS Azure Search postcodes and places filtering on LocalType Postcode when searching for a full postcode with a space', async () => {
      // setup
      const expectedPostcodeLookupRequest = Object.assign({}, defaultNHSSearchServiceRequest, {
        url: POSTCODE_LOOKUP_API_URL,
        data: {
          top: 1,
          search: '"SE14 5NQ"',
          count: true,
          filter: 'LocalType eq \'Postcode\'',
        },
      });

      const expectedOrganisationLookupRequest = Object.assign({}, defaultNHSSearchServiceRequest, {
        url: GP_LOOKUP_API_URL,
        data: {
          top: `${GP_LOOKUP_API_RESULTS_LIMIT}`,
          search: undefined,
          queryType: undefined,
          searchFields: undefined,
          select: 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode',
          filter: getGeoDistanceFilter(-10, 20),
          orderby: getGeoDistanceOrderBy(-10, 20),
          count: true,
        },
      });

      axiosMock.mockResolvedValueOnce({
        data: {
          '@odata.count': 1,
          value: [{
            Latitude: -10,
            Longitude: 20,
          }],
        },
      });

      // work
      await NHSSearchService.searchGPPractices('SE14 5NQ');

      // expect
      expect(axiosMock.mock.calls[0][0]).toEqual(expectedPostcodeLookupRequest);
      expect(axiosMock.mock.calls[1][0]).toEqual(expectedOrganisationLookupRequest);
    });

    it('will call NHS Azure Search postcodes and places filtering on LocalType Postcode when searching for a full postcode without a space', async () => {
      // setup
      const expectedPostcodeLookupRequest = Object.assign({}, defaultNHSSearchServiceRequest, {
        url: POSTCODE_LOOKUP_API_URL,
        data: {
          top: 1,
          search: '"SE14 5NQ"',
          count: true,
          filter: 'LocalType eq \'Postcode\'',
        },
      });

      const expectedOrganisationLookupRequest = Object.assign({}, defaultNHSSearchServiceRequest, {
        url: GP_LOOKUP_API_URL,
        data: {
          top: `${GP_LOOKUP_API_RESULTS_LIMIT}`,
          search: undefined,
          queryType: undefined,
          searchFields: undefined,
          select: 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode',
          filter: getGeoDistanceFilter(-10, 20),
          orderby: getGeoDistanceOrderBy(-10, 20),
          count: true,
        },
      });

      axiosMock.mockResolvedValueOnce({
        data: {
          '@odata.count': 1,
          value: [{
            Latitude: -10,
            Longitude: 20,
          }],
        },
      });

      // work
      await NHSSearchService.searchGPPractices('SE145NQ');

      // expect
      expect(axiosMock.mock.calls[0][0]).toEqual(expectedPostcodeLookupRequest);
      expect(axiosMock.mock.calls[1][0]).toEqual(expectedOrganisationLookupRequest);
    });

    it('will call NHS Azure Search postcodes and places filtering on Type PostcodeOutCode when searching for an outcode', async () => {
      // setup
      const expectedPostcodeLookupRequest = Object.assign({}, defaultNHSSearchServiceRequest, {
        url: POSTCODE_LOOKUP_API_URL,
        data: {
          top: 1,
          search: '"SE14"',
          count: true,
          filter: 'Type eq \'PostcodeOutCode\'',
        },
      });

      axiosMock.mockResolvedValueOnce({
        data: {
          '@odata.count': 1,
          value: [{
            Latitude: -10,
            Longitude: 20,
          }],
        },
      });

      const expectedOrganisationLookupRequest = Object.assign({}, defaultNHSSearchServiceRequest, {
        url: GP_LOOKUP_API_URL,
        data: {
          top: `${GP_LOOKUP_API_RESULTS_LIMIT}`,
          search: undefined,
          queryType: undefined,
          searchFields: undefined,
          select: 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode',
          filter: getGeoDistanceFilter(-10, 20),
          orderby: getGeoDistanceOrderBy(-10, 20),
          count: true,
        },
      });

      // work
      await NHSSearchService.searchGPPractices('SE14');

      // expect
      expect(axiosMock.mock.calls[0][0]).toEqual(expectedPostcodeLookupRequest);
      expect(axiosMock.mock.calls[1][0]).toEqual(expectedOrganisationLookupRequest);
    });

    it('will call NHS Azure Search organisations lookup when searching for a non-postcode', async () => {
      // setup
      const expectedOrganisationLookupRequest = Object.assign({}, defaultNHSSearchServiceRequest, {
        url: GP_LOOKUP_API_URL,
        data: {
          top: `${GP_LOOKUP_API_RESULTS_LIMIT}`,
          search: 'Yorkshire*',
          searchFields: ORGANISATIONS_SEARCH_FIELDS,
          select: ORGANISATIONS_SELECT_FIELDS,
          filter: ORGANISATIONS_TYPE_ID_FILTER,
          queryType: ORGANISATIONS_QUERY_TYPE,
          count: true,
        },
      });

      // work
      await NHSSearchService.searchGPPractices('Yorkshire');

      // expect
      expect(axiosMock.mock.calls[0][0]).toEqual(expectedOrganisationLookupRequest);
    });

    it('will return a queryError error when called with an empty string', async () => {
      // work
      const results = await NHSSearchService.searchGPPractices('');

      // expect
      expect(results).toEqual({ queryError: true });
    });

    it('will return a noResultsFound error when no results are found for a postcode', async () => {
      // setup
      axiosMock.mockResolvedValueOnce({
        data: {
          '@odata.count': 0,
          value: undefined,
        },
      });
      axiosMock.mockResolvedValueOnce({
        data: {
          '@odata.count': 0,
          value: [],
        },
      });
      const noResultsFoundError = { noResultsFound: true };

      // work
      const result1 = await NHSSearchService.searchGPPractices('SE14');
      const result2 = await NHSSearchService.searchGPPractices('SE14');

      // expect
      expect(result1).toEqual(noResultsFoundError);
      expect(result2).toEqual(noResultsFoundError);
    });

    it('will return a postcodeSearchError when there is an error searching for a postcode', async () => {
      // setup
      axiosMock.mockRejectedValueOnce({});

      // work
      const results = await NHSSearchService.searchGPPractices('SE14');

      // expect
      expect(results).toEqual({ postcodeSearchError: true });
    });
  });
});
