/* eslint-disable dot-notation, global-require */
import axios from 'axios';

let consola;
if (process.server) {
  consola = require('consola');
}

const getGeoDistanceFilter = (lat, lon) => `OrganisationTypeID eq 'GPB' and geo.distance(Geocode, geography'POINT(${lon} ${lat})') le ${process.env['POSTCODE_LOOKUP_SEARCH_RADIUS_KM']}`;
const getGeoDistanceOrderBy = (lat, lon) => `geo.distance(Geocode, geography'POINT(${lon} ${lat})')`;
const getPostcodesAndPlacesFilter = outcodeOnly => (outcodeOnly ? 'Type eq \'PostcodeOutCode\'' : 'LocalType eq \'Postcode\'');

const getDefaultOrganisationSearchData = limit => ({
  top: limit,
  search: undefined,
  searchFields: 'OrganisationName,Address2,Address3,City',
  select: 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode',
  filter: 'OrganisationTypeID eq \'GPB\'',
  queryType: 'simple',
  count: true,
});
const POSTCODE_SEARCH_DATA = {
  top: 1,
  search: undefined,
  count: true,
  filter: undefined,
};

const sanitiseSearch = searchQuery => searchQuery.trim().replace(/[-]/g, ' ').replace(/\s\s+/g, ' ').replace(/[/\\^$*+&?,.()|[\]{}"~:!<>£;@%^'`]/g, '');
const prepareSearch = (searchQuery, matchWhole = false) => {
  if (matchWhole) {
    return `"${searchQuery}"`;
  }
  const parts = searchQuery.split(' ').join('*+');
  return `${parts}*`;
};

const getNHSSearchServiceRequest = (data, url) => {
  const headers = {
    'subscription-key': process.env['GP_LOOKUP_API_KEY'],
    'Content-Type': 'application/json',
  };
  return axios({ url, headers, data, method: 'POST' });
};

const getOrganisationSearchRequest = (query) => {
  const search = prepareSearch(query);
  const data = getDefaultOrganisationSearchData(process.env['GP_LOOKUP_API_RESULTS_LIMIT']);
  data.search = search;

  return getNHSSearchServiceRequest(data, process.env['GP_LOOKUP_API_URL']);
};

const getPostcodeSearchRequest = (postcode, outcodeOnly) => {
  const search = prepareSearch(postcode, true);
  let data = POSTCODE_SEARCH_DATA;
  data.filter = getPostcodesAndPlacesFilter(outcodeOnly);
  data.search = search;

  return getNHSSearchServiceRequest(data, process.env['POSTCODE_LOOKUP_API_URL'])
    .then((response) => {
      if (response.data.value && response.data.value.length > 0) {
        const { Latitude, Longitude } = response.data.value[0];
        data = getDefaultOrganisationSearchData(process.env['GP_LOOKUP_API_RESULTS_LIMIT']);
        data.searchFields = undefined;
        data.queryType = undefined;
        data.search = undefined;
        data.filter = getGeoDistanceFilter(Latitude, Longitude);
        data.orderby = getGeoDistanceOrderBy(Latitude, Longitude);
        return getNHSSearchServiceRequest(data, process.env['GP_LOOKUP_API_URL']);
      }
      return { noResultsFound: true };
    })
    .catch((error) => {
      if (consola) {
        consola.error(new Error(`Error searching for Postcode: response: ${error}`));
      }
      return { postcodeSearchError: true };
    });
};

export default {
  searchGPPractices: (searchQuery) => {
    const query = sanitiseSearch(searchQuery);
    if (!query) {
      if (consola) {
        consola.error(new Error('Error searching for GP practice: Empty query sent to NHS Search Service'));
      }
      return { queryError: true };
    }

    const results = new RegExp(/^((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))( ?[0-9][A-Za-z]{2})?)$/g).exec(query);
    if (results) {
      let [postcode] = results;
      const outwardCode = results[2];
      let inwardCode = results[9];

      if (inwardCode && inwardCode.length === 3) {
        inwardCode = ` ${inwardCode}`;
        postcode = `${outwardCode}${inwardCode}`;
      }
      return getPostcodeSearchRequest(postcode, !inwardCode);
    }

    return getOrganisationSearchRequest(query);
  },
};
