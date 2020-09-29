import {
  IS_INCOMPATIBLE,
} from './mutation-types';

export default {
  updateCompatibility({ commit }, isIncompatible) {
    commit(IS_INCOMPATIBLE, isIncompatible);
  },
};
