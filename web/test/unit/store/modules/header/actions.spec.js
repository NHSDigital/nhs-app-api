import actions from '@/store/modules/header/actions';
import { UPDATE_HEADER_TEXT, TOGGLE_MINI_MENU, CLOSE_MINI_MENU, UPDATE_HEADER_CAPTION } from '@/store/modules/header/mutation-types';

const { updateHeaderText, updateHeaderCaption, toggleMiniMenu, closeMiniMenu } = actions;

describe('tests header/action.js', () => {
  describe('updateHeaderText', () => {
    it('will call commit with the sent value', () => {
      const newHeaderValue = 'new page header';

      const commit = jest.fn();

      updateHeaderText({ commit }, newHeaderValue);

      expect(commit).toBeCalledWith(UPDATE_HEADER_TEXT, newHeaderValue);
    });
  });

  describe('updateHeaderCaption', () => {
    it('will call commit with the sent value', () => {
      const newHeaderCaption = 'new header caption';

      const commit = jest.fn();

      updateHeaderCaption({ commit }, newHeaderCaption);

      expect(commit).toBeCalledWith(UPDATE_HEADER_CAPTION, newHeaderCaption);
    });
  });

  describe('toggleMiniMenu', () => {
    it('will call commit with the sent value', () => {
      const commit = jest.fn();

      toggleMiniMenu({ commit });

      expect(commit).toBeCalledWith(TOGGLE_MINI_MENU);
    });
  });

  describe('closeMiniMenu', () => {
    it('will call commit with the sent value', () => {
      const commit = jest.fn();

      closeMiniMenu({ commit });

      expect(commit).toBeCalledWith(CLOSE_MINI_MENU);
    });
  });
});
