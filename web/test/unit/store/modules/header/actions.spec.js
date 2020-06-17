import actions from '@/store/modules/header/actions';
import { TOGGLE_MINI_MENU, CLOSE_MINI_MENU } from '@/store/modules/header/mutation-types';

const { toggleMiniMenu, closeMiniMenu } = actions;

describe('tests header/action.js', () => {
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
