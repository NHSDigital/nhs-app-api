import {
  IS_INCOMPATIBLE,
} from './mutation-types';

export default {
  [IS_INCOMPATIBLE](state, isIncompatible) {
    state.isIncompatible = isIncompatible;
  },
};
