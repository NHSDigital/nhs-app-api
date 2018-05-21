export const CLEAR_SELECTED_MENUITEM = 'CLEAR_SELECTED_MENUITEM';
export const SET_NEWMENUITEM = 'SET_NEWMENUITEM';
const INIT_NAVIGATION = 'INIT_NAVIGATION';

export const state = () => ({
  previousSelectedMenuItem: null,
});
/* eslint-disable no-shadow */
function clearPreviousSelectedMenuItem(state) {
  if (
    state.previousSelectedMenuItem &&
    state.previousSelectedMenuItem.classList
  ) {
    state.previousSelectedMenuItem.classList.remove('active');
  }
}
export const actions = {
  clearPreviousSelectedMenuItem({ commit }) {
    commit(CLEAR_SELECTED_MENUITEM);
  },
  init({ commit }) {
    commit(INIT_NAVIGATION);
  },
  setNewMenuItem({ commit }, menuItem) {
    commit(SET_NEWMENUITEM, menuItem);
  },
};

/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export const mutations = {
  [CLEAR_SELECTED_MENUITEM](state) {
    clearPreviousSelectedMenuItem(state);
  },
  [INIT_NAVIGATION](state) {
    state = {
      previousSelectedMenuItem: null,
    };
  },
  [SET_NEWMENUITEM](state, menuItem) {
    clearPreviousSelectedMenuItem(state);
    menuItem.classList.add('active');
    state.previousSelectedMenuItem = menuItem;
  },
};
