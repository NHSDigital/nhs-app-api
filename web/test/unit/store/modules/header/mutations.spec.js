import mutations from '@/store/modules/header/mutations';
import {
  UPDATE_HEADER_TEXT,
  TOGGLE_MINI_MENU,
  CLOSE_MINI_MENU,
  UPDATE_HEADER_CAPTION,
  INIT_HEADER,
} from '@/store/modules/header/mutation-types';

describe('tests header/mutations.js', () => {
  describe('UPDATE_HEADER_TEXT', () => {
    it('will set the header text on the state to the sent value', () => {
      const state = {};
      const newHeaderText = 'new page header';

      mutations[UPDATE_HEADER_TEXT](state, newHeaderText);

      expect(state.headerText).toEqual(newHeaderText);
    });
  });
  describe('UPDATE_HEADER_CAPTION', () => {
    it('will set the header caption on the state to the sent value', () => {
      const state = {};
      const caption = 'header caption';

      mutations[UPDATE_HEADER_CAPTION](state, caption);

      expect(state.headerCaption).toEqual(caption);
    });
  });

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

  describe('INIT_HEADER', () => {
    it('will clear the header text and caption', () => {
      const state = { headerText: 'header', headerCaption: 'caption' };

      mutations[INIT_HEADER](state);

      expect(state.headerText).toBe('');
      expect(state.headerCaption).toBe('');
    });
  });
});
