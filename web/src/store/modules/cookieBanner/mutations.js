/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
import {
  ACKNOWLEDGE,
  SYNC,
} from './mutation-types';

export default {
  [ACKNOWLEDGE](state) {
    state.acknowledged = true;
  },
  [SYNC](state) {
    state.acknowledged = state.acknowledged;
  },
};
