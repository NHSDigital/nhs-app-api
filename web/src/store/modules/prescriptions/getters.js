import get from 'lodash/fp/get';

export default {
  pharmacyName(state) {
    return get('pharmacy.pharmacyName')(state);
  },
};
