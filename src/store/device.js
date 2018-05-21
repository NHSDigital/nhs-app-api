export const UPDATE_IS_NATIVE_APP = 'UPDATE_IS_NATIVE_APP';
export const INIT_DEVICE = 'INIT_DEVICE';
const initialState = {
  isNativeApp: false,
};
export const state = () => initialState;

export const actions = {
  init({ commit }) {
    commit(INIT_DEVICE);
  },
  updateIsNativeApp({ commit }, isNativeApp) {
    commit(UPDATE_IS_NATIVE_APP, isNativeApp);
  },
};
/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [UPDATE_IS_NATIVE_APP](state, isNativeApp) {
    state.isNativeApp = isNativeApp;
  },
  [INIT_DEVICE](state) {
    state = initialState;
  },
};
