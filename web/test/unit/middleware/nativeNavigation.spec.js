import nativeNavigation from '@/middleware/nativeNavigation';
import { LOGOUT } from '@/router/routes/logout';
import { LOGIN } from '@/router/routes/login';
import { PRESCRIPTIONS } from '@/router/routes/prescriptions';
import { get } from 'lodash/fp';

describe('nativeNavigation middleware', () => {
  let next;
  let store;

  beforeEach(() => {
    next = jest.fn();
    store = {
      dispatch: jest.fn(),
    };
  });

  describe('nativeNavigation field not defined', () => {
    beforeEach(() => {
      nativeNavigation({
        to: LOGOUT,
        store,
        next,
      });
    });

    it('will exit', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });

  describe('nativeNavigation field === CLEAR_SELECTED_MENU_ITEM', () => {
    beforeEach(() => {
      nativeNavigation({
        to: LOGIN,
        store,
        next,
      });
    });

    it('will dispatch "navigation/clearPreviousSelectedMenuItem"', () => {
      expect(store.dispatch).toBeCalledWith('navigation/clearPreviousSelectedMenuItem');
    });

    it('will exit', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });

  describe('nativeNavigation field !== CLEAR_SELECTED_MENU_ITEM', () => {
    beforeEach(() => {
      nativeNavigation({
        to: PRESCRIPTIONS,
        store,
        next,
      });
    });

    it('will dispatch "navigation/clearPreviousSelectedMenuItem"', () => {
      const itemId = get('meta.nativeNavigation')(PRESCRIPTIONS);
      expect(store.dispatch).toBeCalledWith('navigation/setNewMenuItem', itemId);
    });

    it('will exit', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });
});
