import mutations from '@/store/modules/header/mutations';
import {
  TOGGLE_MINI_MENU,
  CLOSE_MINI_MENU,
} from '@/store/modules/header/mutation-types';

describe('TOGGLE_MINI_MENU', () => {
  it('will toggle the state of the mini menu', () => {
    const state = {};

    mutations[TOGGLE_MINI_MENU](state);

    expect(state.miniMenuExpanded).toBe(true);

    mutations[TOGGLE_MINI_MENU](state);

    expect(state.miniMenuExpanded).toBe(false);
  });
});

describe('CLOSE_MINI_MENU', () => {
  it('will set state of the mini menu to collapsed', () => {
    const state = { miniMenuExpanded: true };

    mutations[CLOSE_MINI_MENU](state);

    expect(state.miniMenuExpanded).toBe(false);
  });
});

