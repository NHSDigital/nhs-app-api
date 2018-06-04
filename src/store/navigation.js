export const CLEAR_SELECTED_MENUITEM = 'CLEAR_SELECTED_MENUITEM';
export const SET_NEWMENUITEM = 'SET_NEWMENUITEM';

export const state = () => ({
  menuItemStatusAt: [],

});

/* eslint-disable no-shadow */
function clearPreviousSelectedMenuItem(state) {
  let i;
  const x = [];
  for (i = 0; i < 5; i += 1) {
    x.push(false);
  }
  state.menuItemStatusAt = x;
}
export const actions = {
  clearPreviousSelectedMenuItem({ commit }) {
    commit(CLEAR_SELECTED_MENUITEM);
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
  [SET_NEWMENUITEM](state, menuItemIndex) {
    clearPreviousSelectedMenuItem(state);
    state.menuItemStatusAt[menuItemIndex] = true;
  },
};
