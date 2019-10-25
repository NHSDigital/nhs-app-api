import get from 'lodash/fp/get';

export default {
  hasLinkedAccounts(state) {
    return get('config.hasLinkedAccounts')(state);
  },
};
