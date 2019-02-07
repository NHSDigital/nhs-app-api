import mutations from '@/store/modules/header/mutations';
import { UPDATE_HEADER_TEXT, TOGGLE_MINI_MENU, CLOSE_MINI_MENU } from '@/store/modules/header/mutation-types';

describe('tests header/mutations.js', () => {
  describe('UPDATE_HEADER_TEXT', () => {
    it('will call the native app handle to update the page header', () => {
      const mockUpdateHeaderFunction = jest.fn();
      window.nativeApp = {
        updateHeaderText: mockUpdateHeaderFunction,
      };
      process.client = true;
      const state = {
        headerText: '',
      };
      const newHeaderText = 'new page header';

      mutations[UPDATE_HEADER_TEXT](state, newHeaderText);

      expect(state.headerText).toEqual('new page header');
      expect(mockUpdateHeaderFunction).toHaveBeenCalledWith(newHeaderText);
    });

    it('will set the header text on the state to the sent value when no native app handles are present', () => {
      window.nativeApp = undefined;
      const state = {};
      const newHeaderText = 'new page header';

      mutations[UPDATE_HEADER_TEXT](state, newHeaderText);

      expect(state.headerText).toEqual(newHeaderText);
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
    it('will state of the mini menu to collapsed', () => {
      const state = { miniMenuExpanded: true };

      mutations[CLOSE_MINI_MENU](state);

      expect(state.miniMenuExpanded).toBe(false);
    });
  });
});
