import {
  assign,
} from 'lodash/fp';
import {
  NOMINATED_PHARMACY_CLEAR,
  NOMINATED_PHARMACY_LOADED,
} from './mutation-types';

export default {
  [NOMINATED_PHARMACY_LOADED](state, data) {
    const pharmacy = assign({}, data);

    state.pharmacy = pharmacy;
    state.hasLoaded = true;
  },
  [NOMINATED_PHARMACY_CLEAR](state) {
    state.pharmacy = {};
    state.hasLoaded = false;
  },
};
