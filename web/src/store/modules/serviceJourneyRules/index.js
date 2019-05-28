import getters from './getters';
import { initialState } from './mutation-types';

export default {
  namespaced: true,
  state: initialState,
  getters,
};
