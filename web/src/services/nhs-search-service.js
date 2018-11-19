import axios from 'axios';

export const sanitiseSearch = searchQuery => searchQuery.trim().replace(/[-/\\^$*+?.()|[\]{}"~:!]/g, '\\$&');

export default {
  searchGPPractices: (env, searchQuery) => {
    const method = 'POST';
    const url = env.GP_LOOKUP_API_URL;
    const data = {
      top: env.GP_LOOKUP_API_RESULTS_LIMIT,
      search: `"${sanitiseSearch(searchQuery)}"*`,
      searchFields: 'OrganisationName,Postcode,City',
      select: 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode',
      filter: 'OrganisationTypeID eq \'GPB\'',
      orderby: 'OrganisationName',
    };
    const headers = {
      'subscription-key': env.GP_LOOKUP_API_KEY,
      'Content-Type': 'application/json',
    };
    return axios({ url, method, headers, data });
  },
};
