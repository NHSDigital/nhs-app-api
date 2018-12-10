/* eslint-disable dot-notation */
import axios from 'axios';

export const sanitiseSearch = searchQuery => searchQuery.trim().replace(/[-]/g, ' ').replace(/[/\\^$*+&?,.()|[\]{}"~:!]/g, '');
export const formatSearch = (searchQuery) => {
  const parts = searchQuery.split(' ').join('*+');
  return `${parts}*`;
};

export default {
  searchGPPractices: (searchQuery) => {
    const method = 'POST';
    const url = process.env['GP_LOOKUP_API_URL'];
    const data = {
      top: process.env['GP_LOOKUP_API_RESULTS_LIMIT'],
      search: formatSearch(sanitiseSearch(searchQuery)),
      searchFields: 'OrganisationName,Postcode,City',
      select: 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode',
      filter: 'OrganisationTypeID eq \'GPB\'',
      orderby: 'OrganisationName',
      queryType: 'simple',
      count: true,
    };
    const headers = {
      'subscription-key': process.env['GP_LOOKUP_API_KEY'],
      'Content-Type': 'application/json',
    };
    return axios({ url, method, headers, data });
  },
};
