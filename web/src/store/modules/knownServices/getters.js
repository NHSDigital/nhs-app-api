import { getOr, head } from 'lodash/fp';

export default {
  matchOneByUrl(state) {
    return (url) => {
      const foundServices = getOr([], 'knownServices', state).filter(service => url.startsWith(service.url));
      return head(foundServices);
    };
  },
  matchOneById(state) {
    return (id) => {
      const foundServices = getOr([], 'knownServices', state).filter(service => id === service.id);
      return head(foundServices);
    };
  },
};
