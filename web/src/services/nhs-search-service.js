import axios from 'axios';

export default {

  searchGPPractices: (context) => {
    const method = 'POST';
    const url = context.store.app.$env.GP_LOOKUP_API_URL;
    const data = {
      top: context.store.app.$env.GP_LOOKUP_API_RESULTS_LIMIT,
      search: `${context.route.query.searchQuery}*`,
      searchFields: 'OrganisationName,Postcode,City',
      select: 'OrganisationID,OrganisationName,Address1,Address2,Address3,City,County,Postcode,NACSCode',
      filter: 'OrganisationTypeID eq \'GPB\'',
      orderby: 'OrganisationName',
    };
    const headers = {
      'Ocp-Apim-Subscription-Key': context.store.app.$env.GP_LOOKUP_API_KEY,
      'Content-Type': 'application/json',
    };
    return axios({ url, method, headers, data });
  },

};
